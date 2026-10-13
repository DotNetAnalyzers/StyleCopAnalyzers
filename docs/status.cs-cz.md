<div class="rule-status">
    <style>
        .rule-status .rule-status-table { min-width: 72rem; }
        .rule-status .rule-status-table th:nth-child(3),
        .rule-status .rule-status-table td:nth-child(3) { min-width: 18rem; }
    </style>
    <div id="statusMessage" class="alert alert-info" role="status" aria-live="polite">Načítá se stav pravidel...</div>
    <noscript><p class="alert alert-warning">K zobrazení sestavy stavu pravidel je vyžadován JavaScript.</p></noscript>
    <div class="table-responsive">
        <table class="table table-hover table-sm rule-status-table">
            <caption>Implementace pravidel, opravy kódu a podpora hromadného použití oprav</caption>
            <thead>
                <tr>
                    <th scope="col">Kategorie</th>
                    <th scope="col">ID</th>
                    <th scope="col">Název</th>
                    <th scope="col">Implementováno</th>
                    <th scope="col">Stav</th>
                    <th scope="col">Oprava kódu</th>
                    <th scope="col">Poskytovatel hromadné opravy</th>
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
            <td>{{>~label(Category.replace("StyleCop.CSharp.", ""))}}</td>
            <!-- Runtime template URL. -->
            <!-- mlc-disable-next-line -->
            <td><a href="{{>Id}}.html">{{>Id}}</a></td>
            <td>{{>Title}}</td>
            <td>{{if HasImplementation}}Ano{{else}}Ne{{/if}}</td>
            <td>{{>~label(Status)}}</td>
            <td>
                {{if CodeFixStatus === "Implemented"}}
                <span class="text-success" title="Implementováno"><i class="bi bi-check-circle-fill" aria-hidden="true"></i><span class="visually-hidden">Implementováno</span></span>
                {{else CodeFixStatus === "NotImplemented"}}
                <span class="text-danger" title="Nelze použít: {{>NoCodeFixReason}}"><i class="bi bi-x-circle-fill" aria-hidden="true"></i><span class="visually-hidden">Nelze použít: {{>NoCodeFixReason}}</span></span>
                {{else}}
                <span class="text-primary" title="Zatím neimplementováno"><i class="bi bi-exclamation-circle-fill" aria-hidden="true"></i><span class="visually-hidden">Zatím neimplementováno</span></span>
                {{/if}}
            </td>
            <td>
                {{if FixAllStatus === "BatchFixer"}}
                <span class="text-primary" title="Hromadná oprava"><i class="bi bi-hourglass-split" aria-hidden="true"></i><span class="visually-hidden">Hromadná oprava</span></span>
                {{else FixAllStatus === "CustomImplementation"}}
                <span class="text-success" title="Vlastní implementace"><i class="bi bi-check-circle-fill" aria-hidden="true"></i><span class="visually-hidden">Vlastní implementace</span></span>
                {{else}}
                <span class="text-danger" title="Není k dispozici"><i class="bi bi-x-circle-fill" aria-hidden="true"></i><span class="visually-hidden">Není k dispozici</span></span>
                {{/if}}
            </td>
        </tr>
    </script>
    <script id="diagnostics" type="text/x-jsrender">
        {{for diagnostics tmpl="#diagnostic"/}}
    </script>
    <script id="commitInfo" type="text/x-jsrender">
        <h2 id="commit-information">Informace o revizi</h2>
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
                        <th scope="row">Zpráva</th>
                        <td>{{>Message}}</td>
                    </tr>
                    <tr>
                        <th scope="row">Autor</th>
                        <td>{{>Author.Name}} &lt;{{>Author.Email}}&gt; ({{>Author.When}})</td>
                    </tr>
                    <tr>
                        <th scope="row">Autor revize</th>
                        <td>{{>Committer.Name}} &lt;{{>Committer.Email}}&gt; ({{>Committer.When}})</td>
                    </tr>
                    <tr>
                        <th scope="row">Nadřazené revize</th>
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
                message.textContent = "Nepodařilo se načíst skripty stránky. Zkontrolujte připojení a stránku obnovte.";
                return;
            }
            var labels = {
                SpecialRules: "Speciální pravidla",
                SpacingRules: "Pravidla mezer",
                ReadabilityRules: "Pravidla čitelnosti",
                OrderingRules: "Pravidla řazení",
                NamingRules: "Pravidla pojmenování",
                MaintainabilityRules: "Pravidla udržovatelnosti",
                LayoutRules: "Pravidla rozložení",
                DocumentationRules: "Pravidla dokumentace",
                DisabledNoTests: "Zakázáno: chybí testy",
                DisabledAlternative: "Zakázáno: alternativní pravidlo",
                EnabledByDefault: "Ve výchozím nastavení povoleno",
                DisabledByDefault: "Ve výchozím nastavení zakázáno"
            };
            $.views.helpers({ label: function (value) { return labels[value] || value; } });
            function showError(message, error) {
                console.error(message, error);
                $("#statusMessage").removeClass("alert-info").addClass("alert-danger").text(message).show();
            }
            $.ajax({
                url: "status/StyleCop.Analyzers.Status.cs-CZ.json",
                dataType: "json",
                timeout: 15000
            }).done(function (data) {
                if (!data || !Array.isArray(data.diagnostics) || data.diagnostics.length === 0 || !data.git || !data.git.Sha) {
                    showError("Sestava stavu pravidel je neplatná nebo prázdná. Zkuste to znovu po příštím úspěšném sestavení.", data);
                    return;
                }
                try {
                    var diagnostics = $.templates($("#diagnostics").html()).render(data);
                    var commitInfo = $.templates($("#commitInfo").html()).render(data.git);
                    $("#renderedDiagnostics").html(diagnostics);
                    $("#renderedCommitInfo").html(commitInfo);
                    $("#statusMessage").hide();
                } catch (error) {
                    showError("Sestava stavu pravidel je neplatná nebo prázdná. Zkuste to znovu po příštím úspěšném sestavení.", error);
                }
            }).fail(function (request, status, error) {
                showError("Nepodařilo se načíst sestavu stavu pravidel. Zkontrolujte připojení a stránku obnovte.", error || status);
            });
        })();
    </script>
</div>
