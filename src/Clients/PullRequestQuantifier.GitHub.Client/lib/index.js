"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
var os_1 = __importDefault(require("os"));
var child_process_1 = __importDefault(require("child_process"));
var fs_1 = __importDefault(require("fs"));
var path_1 = __importDefault(require("path"));
var pure_uuid_1 = __importDefault(require("pure-uuid"));
var rimraf_1 = __importDefault(require("rimraf"));
function quantify(context) {
    return __awaiter(this, void 0, void 0, function () {
        var labels, owner, repo, pull_number, diff, quantifierInput, quantifierContext, error_1, id, directory, quantifierInputFile, execCmd, quantifierContextFile;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    labels = new Set();
                    owner = context.payload.repository.owner.login;
                    repo = context.payload.repository.name;
                    pull_number = context.payload.number;
                    return [4 /*yield*/, context.octokit.pulls.listFiles({
                            owner: owner,
                            repo: repo,
                            pull_number: pull_number
                        })];
                case 1:
                    diff = _a.sent();
                    quantifierInput = convertToQuantifierInput(diff);
                    quantifierContext = undefined;
                    _a.label = 2;
                case 2:
                    _a.trys.push([2, 4, , 5]);
                    return [4 /*yield*/, context.octokit.repos.getContent({
                            owner: owner,
                            repo: repo,
                            path: ".prquantifier"
                        })];
                case 3:
                    quantifierContext = (_a.sent()).data.content;
                    quantifierContext = Buffer.from(quantifierContext, 'base64').toString();
                    return [3 /*break*/, 5];
                case 4:
                    error_1 = _a.sent();
                    return [3 /*break*/, 5];
                case 5:
                    id = new pure_uuid_1.default(4).format();
                    directory = path_1.default.join(os_1.default.tmpdir(), "pullRequestQuantifier-" + id);
                    return [4 /*yield*/, fs_1.default.mkdirSync(directory)];
                case 6:
                    _a.sent();
                    try {
                        quantifierInputFile = path_1.default.join(directory, "quantifierInput.json");
                        fs_1.default.writeFile(quantifierInputFile, JSON.stringify(quantifierInput), function (err) {
                            if (err)
                                throw err;
                        });
                        execCmd = "dotnet " + path_1.default.join(__dirname, "PullRequestQuantifier.GitHub.Client.dll") + " -quantifierInput " + quantifierInputFile;
                        if (quantifierContext) {
                            quantifierContextFile = path_1.default.join(directory, "context.yml");
                            fs_1.default.writeFile(quantifierContextFile, quantifierContext, function (err) {
                                if (err)
                                    throw err;
                            });
                            console.log(quantifierContext);
                            console.log(quantifierContextFile);
                            execCmd += " -contextpath " + quantifierContextFile;
                        }
                        // run quantifier process
                        child_process_1.default.exec(execCmd, function (error, stdout, stderr) {
                            if (error) {
                                throw new Error(error);
                            }
                            if (stderr) {
                                throw new Error(stderr);
                            }
                            var label = stdout.split("\t")[0].split("=")[1].trim();
                            labels.add(label);
                            context.octokit.issues.addLabels(context.issue({
                                labels: Array.from(labels)
                            }));
                            // add comment to PR
                            context.octokit.issues.createComment(context.issue({
                                body: stdout
                            }));
                        });
                    }
                    catch (error) {
                        rimraf_1.default.sync(directory);
                        throw error;
                    }
                    return [2 /*return*/];
            }
        });
    });
}
function convertToQuantifierInput(diff) {
    var changes = [];
    diff.data.forEach(function (patch) {
        // TODO: If patch is undefined (for large files), get content separately
        if (!patch.patch) {
            return;
        }
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
        var change = {
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
module.exports = function (_a) {
    var app = _a.app;
    app.on("pull_request.opened", quantify);
    app.on("pull_request.synchronize", quantify);
    app.on("pull_request.reopened", quantify);
};
//# sourceMappingURL=index.js.map