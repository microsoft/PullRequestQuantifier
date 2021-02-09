import * as vscode from "vscode";
import * as fs from "fs";
import { parse } from "path";
var exec = require("child_process").execFile;

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext): void {

	let outputTemplate: string = fs.readFileSync(context.asAbsolutePath("./out/ConsoleOutput.mustache")).toString();
	outputTemplate = escapeRegExp(outputTemplate);
	outputTemplate = outputTemplate.replace(/{{(.*?)}}/g, "(.*)");
	const outputRegex: RegExp = new RegExp(outputTemplate);

	const exePath: string = context.asAbsolutePath("./out/PullRequestQuantifier.VsCode.Client.exe");
	const statusBarItem: vscode.StatusBarItem = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Right, 999);
	vscode.workspace.onDidSaveTextDocument((e: vscode.TextDocument) => {
		exec(exePath, ["-gitrepopath", e.fileName], (_err: any, stdout: string, _stderr: any) => {
			const consoleOutput: string = stdout.trim();
			let parsedOutput: RegExpExecArray | null = outputRegex.exec(consoleOutput);
			if (parsedOutput) {
				const shortOutput: string = `${parsedOutput[1]}, +${parsedOutput[2]} -${parsedOutput[3]}, ${parsedOutput[7]}%`;

				statusBarItem.hide();
				statusBarItem.text = shortOutput.trim();
				statusBarItem.show();
			}
		});
	});
}

function escapeRegExp(text: string) {
	return text.replace(/[.*+?^$()|[\]\\]/g, "\\$&"); // $& means the whole matched string
}

// this method is called when your extension is deactivated
export function deactivate(): void { }
