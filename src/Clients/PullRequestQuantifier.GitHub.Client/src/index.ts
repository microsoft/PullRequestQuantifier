import { WebhookEvent, EventPayloads } from "@octokit/webhooks";
import { Context, Probot } from "probot";
import os from "os";
import process from "child_process";
import fs from "fs";
import path from "path";
import UUID from "pure-uuid";
import rimraf from "rimraf";

export = ({ app }: { app: Probot }) => {
  app.on("pull_request.opened", quantify);
  app.on("pull_request.synchronize", quantify);
  app.on("pull_request.reopened", quantify);
};

async function quantify(context: WebhookEvent<EventPayloads.WebhookPayloadPullRequest> & Pick<Context<any>, "octokit" | "log" | "repo" | "issue" | "pullRequest" | "isBot" | "config" | "event" | "github">) {
  const labels = new Set<string>();

  const owner = context.payload.repository.owner.login
  const repo = context.payload.repository.name
  const pull_number = context.payload.number

  // get diff
  const diff = await context.octokit.pulls.listFiles({
    owner,
    repo,
    pull_number
  });
  var quantifierInput: any = convertToQuantifierInput(diff);

  // get quantifier context if available
  var quantifierContext = undefined;
  try {
    quantifierContext = (await context.octokit.repos.getContent({
      owner,
      repo,
      path: ".prquantifier"
    })).data.content;
    quantifierContext = Buffer.from(quantifierContext, 'base64').toString();
  } catch (error) {
    // no context found, use default context
  }

  // write quantifier input and context to a local file
  const id = new UUID(4).format();
  const directory = path.join(os.tmpdir(), `pullRequestQuantifier-${id}`);
  await fs.mkdirSync(directory);

  try {
    const quantifierInputFile = path.join(directory, "quantifierInput.json");
    fs.writeFile(quantifierInputFile, JSON.stringify(quantifierInput), function (err: any) {
      if (err) throw err;
    });

    var execCmd = `dotnet ${path.join(__dirname, "PullRequestQuantifier.GitHub.Client.dll")} -quantifierInput ${quantifierInputFile}`;
    if (quantifierContext) {
      const quantifierContextFile = path.join(directory, "context.yml");
      fs.writeFile(quantifierContextFile, quantifierContext, function (err: any) {
        if (err) throw err;
      });
      console.log(quantifierContext);
      console.log(quantifierContextFile);
      execCmd += ` -contextpath ${quantifierContextFile}`;
    }

    // run quantifier process
    process.exec(execCmd,
      function (error: any, stdout: string, stderr: any) {
        if (error) {
          throw new Error(error);
        }
        if (stderr) {
          throw new Error(stderr);
        }

        const label = stdout.split("\t")[0].split("=")[1].trim();
        labels.add(label);

        context.octokit.issues.addLabels(context.issue({
          labels: Array.from(labels)
        }));

        // add comment to PR
        context.octokit.issues.createComment(context.issue({
          body: stdout
        }));
      });
  } catch (error) {
    rimraf.sync(directory);
    throw error;
  }
}

function convertToQuantifierInput(diff: any) {
  var changes: any = [];
  diff.data.forEach((patch: any) => {
    var changeType = 3;
    switch (patch.status) {
      case "added":
        changeType = 1;
        break;
      case "deleted":
        changeType = 2;
        break;
      case "modified":
        changeType = 3;
        break;
    }
    const change = {
      AbsoluteLinesAdded: patch.additions,
      AbsoluteLinesDeleted: patch.deletions,
      DiffContent: patch.patch,
      DiffContentLines: patch.patch.split("\n"),
      FilePath: patch.filename,
      ChangeType: changeType
    };
    changes.push(change);
  });
  return changes;
}

