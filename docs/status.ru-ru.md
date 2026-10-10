<div class="rule-status">
    <style>
        .rule-status .rule-status-table { min-width: 72rem; }
        .rule-status .rule-status-table th:nth-child(3),
        .rule-status .rule-status-table td:nth-child(3) { min-width: 18rem; }
    </style>
    <div id="statusMessage" class="alert alert-info" role="status" aria-live="polite">Загрузка состояния правил...</div>
    <noscript><p class="alert alert-warning">Для отображения отчёта о состоянии правил требуется JavaScript.</p></noscript>
    <div class="table-responsive">
        <table class="table table-hover table-sm rule-status-table">
            <caption>Реализация правил, исправлений и поддержки «Исправить все»</caption>
            <thead>
                <tr>
                    <th scope="col">Категория</th>
                    <th scope="col">ID</th>
                    <th scope="col">Название</th>
                    <th scope="col">Реализовано</th>
                    <th scope="col">Состояние</th>
                    <th scope="col">Исправление</th>
                    <th scope="col">Поставщик «Исправить все»</th>
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
            <td>{{if HasImplementation}}Да{{else}}Нет{{/if}}</td>
            <td>{{>~label(Status)}}</td>
            <td>
                {{if CodeFixStatus === "Implemented"}}
                <span class="text-success" title="Реализовано"><i class="bi bi-check-circle-fill" aria-hidden="true"></i><span class="visually-hidden">Реализовано</span></span>
                {{else CodeFixStatus === "NotImplemented"}}
                <span class="text-danger" title="Не применимо: {{>NoCodeFixReason}}"><i class="bi bi-x-circle-fill" aria-hidden="true"></i><span class="visually-hidden">Не применимо: {{>NoCodeFixReason}}</span></span>
                {{else}}
                <span class="text-primary" title="Пока не реализовано"><i class="bi bi-exclamation-circle-fill" aria-hidden="true"></i><span class="visually-hidden">Пока не реализовано</span></span>
                {{/if}}
            </td>
            <td>
                {{if FixAllStatus === "BatchFixer"}}
                <span class="text-primary" title="Пакетное исправление"><i class="bi bi-hourglass-split" aria-hidden="true"></i><span class="visually-hidden">Пакетное исправление</span></span>
                {{else FixAllStatus === "CustomImplementation"}}
                <span class="text-success" title="Собственная реализация"><i class="bi bi-check-circle-fill" aria-hidden="true"></i><span class="visually-hidden">Собственная реализация</span></span>
                {{else}}
                <span class="text-danger" title="Отсутствует"><i class="bi bi-x-circle-fill" aria-hidden="true"></i><span class="visually-hidden">Отсутствует</span></span>
                {{/if}}
            </td>
        </tr>
    </script>
    <script id="diagnostics" type="text/x-jsrender">
        {{for diagnostics tmpl="#diagnostic"/}}
    </script>
    <script id="commitInfo" type="text/x-jsrender">
        <h2 id="commit-information">Сведения о коммите</h2>
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
                        <th scope="row">Сообщение</th>
                        <td>{{>Message}}</td>
                    </tr>
                    <tr>
                        <th scope="row">Автор</th>
                        <td>{{>Author.Name}} &lt;{{>Author.Email}}&gt; ({{>Author.When}})</td>
                    </tr>
                    <tr>
                        <th scope="row">Создатель коммита</th>
                        <td>{{>Committer.Name}} &lt;{{>Committer.Email}}&gt; ({{>Committer.When}})</td>
                    </tr>
                    <tr>
                        <th scope="row">Родительские коммиты</th>
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
                message.textContent = "Не удалось загрузить скрипты страницы. Проверьте подключение и обновите страницу.";
                return;
            }
            var labels = {
                SpecialRules: "Специальные правила",
                SpacingRules: "Правила расстановки пробелов",
                ReadabilityRules: "Правила удобочитаемости",
                OrderingRules: "Правила порядка элементов",
                NamingRules: "Правила именования",
                MaintainabilityRules: "Правила сопровождаемости",
                LayoutRules: "Правила оформления",
                DocumentationRules: "Правила документирования",
                DisabledNoTests: "Отключено: нет тестов",
                DisabledAlternative: "Отключено: альтернативное правило",
                EnabledByDefault: "Включено по умолчанию",
                DisabledByDefault: "Отключено по умолчанию"
            };
            $.views.helpers({ label: function (value) { return labels[value] || value; } });
            function showError(message, error) {
                console.error(message, error);
                $("#statusMessage").removeClass("alert-info").addClass("alert-danger").text(message).show();
            }
            $.ajax({
                url: "status/StyleCop.Analyzers.Status.ru-RU.json",
                dataType: "json",
                timeout: 15000
            }).done(function (data) {
                if (!data || !Array.isArray(data.diagnostics) || data.diagnostics.length === 0 || !data.git || !data.git.Sha) {
                    showError("Отчёт о состоянии правил недопустим или пуст. Повторите попытку после следующей успешной сборки.", data);
                    return;
                }
                try {
                    var diagnostics = $.templates($("#diagnostics").html()).render(data);
                    var commitInfo = $.templates($("#commitInfo").html()).render(data.git);
                    $("#renderedDiagnostics").html(diagnostics);
                    $("#renderedCommitInfo").html(commitInfo);
                    $("#statusMessage").hide();
                } catch (error) {
                    showError("Отчёт о состоянии правил недопустим или пуст. Повторите попытку после следующей успешной сборки.", error);
                }
            }).fail(function (request, status, error) {
                showError("Не удалось загрузить отчёт о состоянии правил. Проверьте подключение и обновите страницу.", error || status);
            });
        })();
    </script>
</div>
