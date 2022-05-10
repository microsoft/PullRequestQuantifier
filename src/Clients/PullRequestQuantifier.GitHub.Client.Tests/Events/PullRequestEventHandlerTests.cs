
        [Fact]
        public async Task HandleEvent_FromNoDiffToDiff_SetNewLabelCorrectly()
        {
            var previousLabelName = "No Changes";
            var previousLabel = new Label(1L, string.Empty, previousLabelName, "1", string.Empty, string.Empty, true);
            var newLabelName = "Extra Small";
            var diff = @"diff --git a/a.txt b/a.txt
new file mode 100644
index 0000000..9daeafb
--- /dev/null
+++ b/a.txt
@@ -0,0 +1 @@
+test";
            var addedFile = new PullRequestFile(string.Empty, "addedFile", string.Empty, 1, 0, 0, string.Empty, string.Empty, string.Empty, diff, "addedFile");

            // setup
            var installationEventHandler = new PullRequestEventHandler(
                gitHubClientAdapterFactory.Object,
                appTelemetry.Object,
                new Mock<ILogger<PullRequestEventHandler>>().Object);

            gitHubClientAdapter.Setup(f => f.GetIssueLabelsAsync(It.IsAny<long>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Label> { previousLabel });

            gitHubClientAdapter.Setup(f => f.GetPullRequestFilesAsync(It.IsAny<long>(), It.IsAny<int>()))
                .ReturnsAsync(new List<PullRequestFile> { addedFile });

            // act
            await installationEventHandler.HandleEvent(await File.ReadAllTextAsync(@"Data/PulRequestPayload.txt"));

            // assert
            gitHubClientAdapter.Verify(
                f =>
                    f.RemoveLabelFromIssueAsync(It.IsAny<long>(), It.IsAny<int>(), previousLabelName),
                Times.Once);

            gitHubClientAdapter.Verify(
                f =>
                    f.ApplyLabelAsync(It.IsAny<long>(), It.IsAny<int>(), new[] { newLabelName }), Times.Once);
        }

        [Fact]
        public async Task HandleEvent_LabelSizeChanges_SetNewLabelCorrectly()
        {
            var previousLabelName = "Small";
            var previousLabel = new Label(1L, string.Empty, previousLabelName, "1", string.Empty, string.Empty, true);
            var newLabelName = "Extra Small";
            var diff = @"diff --git a/a.txt b/a.txt
new file mode 100644
index 0000000..9daeafb
--- /dev/null
+++ b/a.txt
@@ -0,0 +1 @@
+test";

            var addedFile = new PullRequestFile(string.Empty, "addedFile", string.Empty, 1, 0, 0, string.Empty, string.Empty, string.Empty, diff, "addedFile");

            // setup
            var installationEventHandler = new PullRequestEventHandler(
                gitHubClientAdapterFactory.Object,
                appTelemetry.Object,
                new Mock<ILogger<PullRequestEventHandler>>().Object);

            gitHubClientAdapter.Setup(f => f.GetIssueLabelsAsync(It.IsAny<long>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Label> { previousLabel });

            gitHubClientAdapter.Setup(f => f.GetPullRequestFilesAsync(It.IsAny<long>(), It.IsAny<int>()))
                .ReturnsAsync(new List<PullRequestFile> { addedFile });

            // act
            await installationEventHandler.HandleEvent(await File.ReadAllTextAsync(@"Data/PulRequestPayload.txt"));

            // assert
            gitHubClientAdapter.Verify(
                f =>
                    f.RemoveLabelFromIssueAsync(It.IsAny<long>(), It.IsAny<int>(), previousLabelName),
                Times.Once);

            gitHubClientAdapter.Verify(
                f =>
                    f.ApplyLabelAsync(It.IsAny<long>(), It.IsAny<int>(), new[] { newLabelName }), Times.Once);
        }