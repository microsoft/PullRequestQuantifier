import * as vscode from "vscode";
var exec = require("child_process").execFile;

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext): void {

	const exePath: string = context.asAbsolutePath("./out/PullRequestQuantifier.VsCode.Client.exe");
	const statusBarItem: vscode.StatusBarItem = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Right, 999);
	vscode.workspace.onDidSaveTextDocument((e: vscode.TextDocument) => {
		exec(exePath, ["-gitrepopath", e.fileName], (_err: any, stdout: string, _stderr: any) => {
			statusBarItem.hide();
			statusBarItem.text = stdout.trim();
			statusBarItem.show();
		});
	});
}

// this method is called when your extension is deactivated
export function deactivate(): void {}
