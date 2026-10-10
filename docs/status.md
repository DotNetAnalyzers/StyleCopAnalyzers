<div class="rule-status">
    <style>
        .rule-status .rule-status-table { min-width: 72rem; }
        .rule-status .rule-status-table th:nth-child(3),
        .rule-status .rule-status-table td:nth-child(3) { min-width: 18rem; }
    </style>
    <div id="statusMessage" class="alert alert-info" role="status" aria-live="polite">Loading rule status...</div>
    <noscript><p class="alert alert-warning">JavaScript is required to display the rule status report.</p></noscript>
    <div class="table-responsive">
        <table class="table table-hover table-sm rule-status-table">
            <caption>Rule implementations, code fixes, and Fix All support</caption>
            <thead>
                <tr>
                    <th scope="col">Category</th>
                    <th scope="col">ID</th>
                    <th scope="col">Title</th>
                    <th scope="col">Has implementation</th>
                    <th scope="col">Status</th>
                    <th scope="col">Code fix</th>
                    <th scope="col">Fix All provider</th>
                </tr>
            </thead>
            <tbody id="renderedDiagnostics"></tbody>
        </table>
    </div>
    <div id="renderedCommitInfo"></div>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jsrender/0.9.90/jsrender.min.js"></script>
    <script id="diagnostic-color" type="text/x-jsrender">
        {{if HasImplementation}}
        {{if Status !== "DisabledNoTests"}}
        table-success
        {{else}}
        table-warning
        {{/if}}
        {{else}}
        table-danger
        {{/if}}
    </script>
    <script id="diagnostic" type="text/x-jsrender">
        <tr class="{{include tmpl="#diagnostic-color"/}}">
            <td>{{>Category.replace("StyleCop.CSharp.", "")}}</td>
            <!-- Runtime template URL. -->
            <!-- mlc-disable-next-line -->
            <td><a href="{{>Id}}.html">{{>Id}}</a></td>
            <td>{{>Title}}</td>
            <td>{{>HasImplementation}}</td>
            <td>{{>Status}}</td>
            <td>
                {{if CodeFixStatus === "Implemented"}}
                <span class="text-success" title="Implemented"><i class="bi bi-check-circle-fill" aria-hidden="true"></i><span class="visually-hidden">Implemented</span></span>
                {{else CodeFixStatus === "NotImplemented"}}
                <span class="text-danger" title="Not applicable: {{>NoCodeFixReason}}"><i class="bi bi-x-circle-fill" aria-hidden="true"></i><span class="visually-hidden">Not applicable: {{>NoCodeFixReason}}</span></span>
                {{else}}
                <span class="text-primary" title="Not yet implemented"><i class="bi bi-exclamation-circle-fill" aria-hidden="true"></i><span class="visually-hidden">Not yet implemented</span></span>
                {{/if}}
            </td>
            <td>
                {{if FixAllStatus === "BatchFixer"}}
                <span class="text-primary" title="Batch fixer"><i class="bi bi-hourglass-split" aria-hidden="true"></i><span class="visually-hidden">Batch fixer</span></span>
                {{else FixAllStatus === "CustomImplementation"}}
                <span class="text-success" title="Custom implementation"><i class="bi bi-check-circle-fill" aria-hidden="true"></i><span class="visually-hidden">Custom implementation</span></span>
                {{else}}
                <span class="text-danger" title="None"><i class="bi bi-x-circle-fill" aria-hidden="true"></i><span class="visually-hidden">None</span></span>
                {{/if}}
            </td>
        </tr>
    </script>
    <script id="diagnostics" type="text/x-jsrender">
        {{for diagnostics tmpl="#diagnostic"/}}
    </script>
    <script id="commitInfo" type="text/x-jsrender">
        <h2 id="commit-information">Commit information</h2>
        <div class="table-responsive">
            <table class="table">
                <tbody>
                    <tr>
                        <th scope="row">SHA</th>
                        <!-- Runtime template URL. -->
                        <!-- mlc-disable-next-line -->
                        <td><a href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/tree/{{>Sha}}">{{>Sha}}</a></td>
                    </tr>
                    <tr>
                        <th scope="row">Message</th>
                        <td>{{>Message}}</td>
                    </tr>
                    <tr>
                        <th scope="row">Author</th>
                        <td>{{>Author.Name}} &lt;{{>Author.Email}}&gt; ({{>Author.When}})</td>
                    </tr>
                    <tr>
                        <th scope="row">Committer</th>
                        <td>{{>Committer.Name}} &lt;{{>Committer.Email}}&gt; ({{>Committer.When}})</td>
                    </tr>
                    <tr>
                        <th scope="row">Parents</th>
                        <td>{{>Parents}}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </script>
    <script type="text/javascript">
        (function () {
            if (!window.jQuery || !window.jQuery.templates) {
                var message = document.getElementById("statusMessage");
                message.className = "alert alert-danger";
                message.textContent = "The page scripts could not be loaded. Check your connection and reload this page.";
                return;
            }
            function showError(message, error) {
                console.error(message, error);
                $("#statusMessage").removeClass("alert-info").addClass("alert-danger").text(message).show();
            }
            $.ajax({
                url: "status/StyleCop.Analyzers.Status.json",
                dataType: "json",
                timeout: 15000
            }).done(function (data) {
                if (!data || !Array.isArray(data.diagnostics) || data.diagnostics.length === 0 || !data.git || !data.git.Sha) {
                    showError("The rule status report is invalid or empty. Please try again after the next successful build.", data);
                    return;
                }
                try {
                    var diagnostics = $.templates($("#diagnostics").html()).render(data);
                    var commitInfo = $.templates($("#commitInfo").html()).render(data.git);
                    $("#renderedDiagnostics").html(diagnostics);
                    $("#renderedCommitInfo").html(commitInfo);
                    $("#statusMessage").hide();
                } catch (error) {
                    showError("The rule status report is invalid or empty. Please try again after the next successful build.", error);
                }
            }).fail(function (request, status, error) {
                showError("The rule status report could not be loaded. Check your connection and reload this page.", error || status);
            });
        })();
    </script>
</div>
