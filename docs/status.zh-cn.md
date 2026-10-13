<div class="rule-status">
    <style>
        .rule-status .rule-status-table { min-width: 72rem; }
        .rule-status .rule-status-table th:nth-child(3),
        .rule-status .rule-status-table td:nth-child(3) { min-width: 18rem; }
    </style>
    <div id="statusMessage" class="alert alert-info" role="status" aria-live="polite">正在加载规则状态...</div>
    <noscript><p class="alert alert-warning">显示规则状态报告需要启用 JavaScript。</p></noscript>
    <div class="table-responsive">
        <table class="table table-hover table-sm rule-status-table">
            <caption>规则实现、代码修复和“全部修复”支持情况</caption>
            <thead>
                <tr>
                    <th scope="col">类别</th>
                    <th scope="col">标识符</th>
                    <th scope="col">标题</th>
                    <th scope="col">已实现</th>
                    <th scope="col">状态</th>
                    <th scope="col">代码修复</th>
                    <th scope="col">“全部修复”提供程序</th>
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
            <!-- 运行时模板 URL。 -->
            <!-- mlc-disable-next-line -->
            <td><a href="{{>Id}}.html">{{>Id}}</a></td>
            <td>{{>Title}}</td>
            <td>{{if HasImplementation}}是{{else}}否{{/if}}</td>
            <td>{{>~label(Status)}}</td>
            <td>
                {{if CodeFixStatus === "Implemented"}}
                <span class="text-success" title="已实现"><i class="bi bi-check-circle-fill" aria-hidden="true"></i><span class="visually-hidden">已实现</span></span>
                {{else CodeFixStatus === "NotImplemented"}}
                <span class="text-danger" title="不适用：{{>~label(NoCodeFixReason)}}"><i class="bi bi-x-circle-fill" aria-hidden="true"></i><span class="visually-hidden">不适用：{{>~label(NoCodeFixReason)}}</span></span>
                {{else}}
                <span class="text-primary" title="尚未实现"><i class="bi bi-exclamation-circle-fill" aria-hidden="true"></i><span class="visually-hidden">尚未实现</span></span>
                {{/if}}
            </td>
            <td>
                {{if FixAllStatus === "BatchFixer"}}
                <span class="text-primary" title="批量修复"><i class="bi bi-hourglass-split" aria-hidden="true"></i><span class="visually-hidden">批量修复</span></span>
                {{else FixAllStatus === "CustomImplementation"}}
                <span class="text-success" title="自定义实现"><i class="bi bi-check-circle-fill" aria-hidden="true"></i><span class="visually-hidden">自定义实现</span></span>
                {{else}}
                <span class="text-danger" title="无"><i class="bi bi-x-circle-fill" aria-hidden="true"></i><span class="visually-hidden">无</span></span>
                {{/if}}
            </td>
        </tr>
    </script>
    <script id="diagnostics" type="text/x-jsrender">
        {{for diagnostics tmpl="#diagnostic"/}}
    </script>
    <script id="commitInfo" type="text/x-jsrender">
        <h2 id="commit-information">提交信息</h2>
        <div class="table-responsive">
            <table class="table">
                <tbody>
                    <tr>
                        <th scope="row">SHA</th>
                        <!-- 运行时模板 URL。 -->
                        <!-- mlc-disable-next-line -->
                        <td><a href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/tree/{{>Sha}}">{{>Sha}}</a></td>
                    </tr>
                    <tr>
                        <th scope="row">消息</th>
                        <td>{{>Message}}</td>
                    </tr>
                    <tr>
                        <th scope="row">作者</th>
                        <td>{{>Author.Name}} &lt;{{>Author.Email}}&gt; ({{>Author.When}})</td>
                    </tr>
                    <tr>
                        <th scope="row">提交者</th>
                        <td>{{>Committer.Name}} &lt;{{>Committer.Email}}&gt; ({{>Committer.When}})</td>
                    </tr>
                    <tr>
                        <th scope="row">父提交</th>
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
                message.textContent = "无法加载页面脚本。请检查网络连接并重新加载此页面。";
                return;
            }
            var labels = {
                SpecialRules: "特殊规则",
                SpacingRules: "空格规则",
                ReadabilityRules: "可读性规则",
                OrderingRules: "排序规则",
                NamingRules: "命名规则",
                MaintainabilityRules: "可维护性规则",
                LayoutRules: "布局规则",
                DocumentationRules: "文档规则",
                DisabledNoTests: "已禁用：没有测试",
                DisabledAlternative: "已禁用：替代规则",
                EnabledByDefault: "默认启用",
                DisabledByDefault: "默认禁用",
                "No automatic code fix is possible for general JSON syntax errors.": "无法自动修复一般的 JSON 语法错误。",
                "The necessary actions for this code fix are not supported by the analysis infrastructure.": "分析基础结构不支持此代码修复所需的操作。",
                "Cannot generate documentation": "无法生成文档",
                "Don't fix what isn't broken.": "无需修复没有问题的代码。",
                "The \"Encapsulate Field\" fix is provided by Visual Studio.": "Visual Studio 提供了“封装字段”修复。",
                "No message is available for Debug.Fail": "没有可用于 Debug.Fail 的消息",
                "Provided by Visual Studio": "由 Visual Studio 提供",
                "Cannot generate appropriate names.": "无法生成合适的名称。"
            };
            $.views.helpers({ label: function (value) { return labels[value] || value; } });
            function showError(message, error) {
                console.error(message, error);
                $("#statusMessage").removeClass("alert-info").addClass("alert-danger").text(message).show();
            }
            $.ajax({
                url: "status/StyleCop.Analyzers.Status.zh-CN.json",
                dataType: "json",
                timeout: 15000
            }).done(function (data) {
                if (!data || !Array.isArray(data.diagnostics) || data.diagnostics.length === 0 || !data.git || !data.git.Sha) {
                    showError("规则状态报告无效或为空。请在下次成功生成后重试。", data);
                    return;
                }
                try {
                    var diagnostics = $.templates($("#diagnostics").html()).render(data);
                    var commitInfo = $.templates($("#commitInfo").html()).render(data.git);
                    $("#renderedDiagnostics").html(diagnostics);
                    $("#renderedCommitInfo").html(commitInfo);
                    $("#statusMessage").hide();
                } catch (error) {
                    showError("规则状态报告无效或为空。请在下次成功生成后重试。", error);
                }
            }).fail(function (request, status, error) {
                showError("无法加载规则状态报告。请检查网络连接并重新加载此页面。", error || status);
            });
        })();
    </script>
</div>
