import ***REMOVED*** WebhookEvent, EventPayloads ***REMOVED*** from "@octokit/webhooks";
import ***REMOVED*** Context, Probot ***REMOVED*** from "probot";
import os from "os";
import process from "child_process";
import fs from "fs";
import path from "path";
import UUID from "pure-uuid";
import rimraf from "rimraf";

export = (***REMOVED*** app ***REMOVED***: ***REMOVED*** app: Probot ***REMOVED***) => ***REMOVED***
  app.on("pull_request.opened", quantify);
  app.on("pull_request.synchronize", quantify);
  app.on("pull_request.reopened", quantify);
***REMOVED***;

async function quantify(context: WebhookEvent<EventPayloads.WebhookPayloadPullRequest> & Pick<Context<any>, "octokit" | "log" | "repo" | "issue" | "pullRequest" | "isBot" | "config" | "event" | "github">) ***REMOVED***
  const labels = new Set<string>();

  const owner = context.payload.repository.owner.login
  const repo = context.payload.repository.name
  const pull_number = context.payload.number

  // get diff
  const diff = await context.octokit.pulls.listFiles(***REMOVED***
    owner,
    repo,
    pull_number
  ***REMOVED***);
  var quantifierInput: any = convertToQuantifierInput(diff);

  // get quantifier context if available
  var quantifierContext = undefined;
  try ***REMOVED***
    quantifierContext = (await context.octokit.repos.getContent(***REMOVED***
      owner,
      repo,
      path: ".prquantifier"
***REMOVED***)).data.content;
    quantifierContext = Buffer.from(quantifierContext, 'base64').toString();
  ***REMOVED*** catch (error) ***REMOVED***
    // no context found, use default context
  ***REMOVED***

  // write quantifier input and context to a local file
  const id = new UUID(4).format();
  const directory = path.join(os.tmpdir(), `pullRequestQuantifier-$***REMOVED***id***REMOVED***`);
  await fs.mkdirSync(directory);

  try ***REMOVED***
    const quantifierInputFile = path.join(directory, "quantifierInput.json");
    fs.writeFile(quantifierInputFile, JSON.stringify(quantifierInput), function (err: any) ***REMOVED***
      if (err) throw err;
***REMOVED***);

    var execCmd = `dotnet $***REMOVED***path.join(__dirname, "PullRequestQuantifier.GitHub.Client.dll")***REMOVED*** -quantifierInput $***REMOVED***quantifierInputFile***REMOVED***`;
    if (quantifierContext) ***REMOVED***
      const quantifierContextFile = path.join(directory, "context.yml");
      fs.writeFile(quantifierContextFile, quantifierContext, function (err: any) ***REMOVED***
        if (err) throw err;
  ***REMOVED***);
      console.log(quantifierContext);
      console.log(quantifierContextFile);
      execCmd += ` -contextpath $***REMOVED***quantifierContextFile***REMOVED***`;
***REMOVED***

    // run quantifier process
    process.exec(execCmd,
      function (error: any, stdout: string, stderr: any) ***REMOVED***
        if (error) ***REMOVED***
          throw new Error(error);
***REMOVED***
        if (stderr) ***REMOVED***
          throw new Error(stderr);
***REMOVED***

        const label = stdout.split("\t")[0].split("=")[1].trim();
        labels.add(label);

        context.octokit.issues.addLabels(context.issue(***REMOVED***
          labels: Array.from(labels)
***REMOVED***));

        // add comment to PR
        context.octokit.issues.createComment(context.issue(***REMOVED***
          body: stdout
***REMOVED***));
  ***REMOVED***);
  ***REMOVED*** catch (error) ***REMOVED***
    rimraf.sync(directory);
    throw error;
  ***REMOVED***
***REMOVED***

function convertToQuantifierInput(diff: any) ***REMOVED***
  var changes: any = [];
  diff.data.forEach((patch: any) => ***REMOVED***
    var changeType = 3;
    switch (patch.status) ***REMOVED***
      case "added":
        changeType = 1;
        break;
      case "deleted":
        changeType = 2;
        break;
      case "modified":
        changeType = 3;
        break;
***REMOVED***
    const change = ***REMOVED***
      AbsoluteLinesAdded: patch.additions,
      AbsoluteLinesDeleted: patch.deletions,
      DiffContent: patch.patch,
      DiffContentLines: patch.patch.split("\n"),
      FilePath: patch.filename,
      ChangeType: changeType
***REMOVED***;
    changes.push(change);
  ***REMOVED***);
  return changes;
***REMOVED***

