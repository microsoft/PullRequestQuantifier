import * as vscode from "vscode";
import * as fs from "fs";
var exec = require("child_process").execFile;

const moreInfoButton: string = "More info";
const moreInfoUrl: string = "https://github.com/microsoft/PullRequestQuantifier";

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext): void {

	let outputTemplate: string = fs.readFileSync(context.asAbsolutePath("./out/ConsoleOutput.mustache")).toString();
	outputTemplate = escapeRegExp(outputTemplate);
	outputTemplate = outputTemplate.replace(/{{(.*?)}}/g, "(.*)");
	const outputRegex: RegExp = new RegExp(outputTemplate);

	const showMoreInfoCommand: string = "prquantifier.showMoreInfo";
	let moreInfoOutput: string = "";
	vscode.commands.registerCommand(showMoreInfoCommand, () => {
		vscode.window.showInformationMessage(moreInfoOutput, moreInfoButton, "Close").then((selectedValue) => {
			if (selectedValue === moreInfoButton) {
				// open link to GitHub repo for more info
				vscode.env.openExternal(vscode.Uri.parse(moreInfoUrl));
			}
		});
	});

	const exePath: string = context.asAbsolutePath("./out/PullRequestQuantifier.VsCode.Client.exe");
	const statusBarItem: vscode.StatusBarItem = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Right, 999);
	statusBarItem.command = showMoreInfoCommand;

	vscode.workspace.onDidSaveTextDocument((e: vscode.TextDocument) => {
		exec(exePath, ["-gitrepopath", e.fileName], (_err: any, stdout: string, _stderr: any) => {
			moreInfoOutput = stdout.trim();
			let parsedOutput: RegExpExecArray | null = outputRegex.exec(moreInfoOutput);
			if (parsedOutput) {
				const shortOutput: string = `$(tag) ${parsedOutput[1]}, +${parsedOutput[2]} -${parsedOutput[3]}, ${parsedOutput[7]}%`;

				statusBarItem.hide();
				statusBarItem.text = shortOutput.trim();
				statusBarItem.show();
			}
		});
	});
}

function escapeRegExp(text: string): string {
	return text.replace(/[.*+?^$()|[\]\\]/g, "\\$&"); // $& means the whole matched string
}

// this method is called when your extension is deactivated
// tslint:disable-next-line: no-empty
export function deactivate(): void { }
