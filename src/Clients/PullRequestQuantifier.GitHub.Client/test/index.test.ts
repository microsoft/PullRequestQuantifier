// You can import your modules
// import index from '../src/index'

import nock from "nock";
// Requiring our app implementation
import myProbotApp from "../src";
import ***REMOVED*** Probot, ProbotOctokit ***REMOVED*** from "probot";
// Requiring our fixtures
import payload from "./fixtures/issues.opened.json";
const issueCreatedBody = ***REMOVED*** body: "Thanks for opening this issue!" ***REMOVED***;
const fs = require("fs");
const path = require("path");

const privateKey = fs.readFileSync(
  path.join(__dirname, "fixtures/mock-cert.pem"),
  "utf-8"
);

describe("My Probot app", () => ***REMOVED***
  let probot: any;

  beforeEach(() => ***REMOVED***
    nock.disableNetConnect();
    probot = new Probot(***REMOVED***
      id: 123,
      privateKey,
      // disable request throttling and retries for testing
      Octokit: ProbotOctokit.defaults(***REMOVED***
        retry: ***REMOVED*** enabled: false ***REMOVED***,
        throttle: ***REMOVED*** enabled: false ***REMOVED***,
  ***REMOVED***),
***REMOVED***);
    // Load our app into probot
    probot.load(myProbotApp);
  ***REMOVED***);

  test("creates a comment when an issue is opened", async (done) => ***REMOVED***
    const mock = nock("https://api.github.com")
      // Test that we correctly return a test token
      .post("/app/installations/2/access_tokens")
      .reply(200, ***REMOVED***
        token: "test",
        permissions: ***REMOVED***
          issues: "write",
***REMOVED***,
  ***REMOVED***)

      // Test that a comment is posted
      .post("/repos/hiimbex/testing-things/issues/1/comments", (body: any) => ***REMOVED***
        done(expect(body).toMatchObject(issueCreatedBody));
        return true;
  ***REMOVED***)
      .reply(200);

    // Receive a webhook event
    await probot.receive(***REMOVED*** name: "issues", payload ***REMOVED***);

    expect(mock.pendingMocks()).toStrictEqual([]);
  ***REMOVED***);

  afterEach(() => ***REMOVED***
    nock.cleanAll();
    nock.enableNetConnect();
  ***REMOVED***);
***REMOVED***);

// For more information about testing with Jest see:
// https://facebook.github.io/jest/

// For more information about using TypeScript in your tests, Jest recommends:
// https://github.com/kulshekhar/ts-jest

// For more information about testing with Nock see:
// https://github.com/nock/nock
