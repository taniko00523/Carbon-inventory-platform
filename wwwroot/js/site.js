// 碳盤查平台 —— 全站前端行為
// 原本這個檔案是空的（只有範本註解），每個 View 各自寫 inline <script>。
// 這裡只放「所有頁面都需要」的漸進增強，不影響任何既有頁面的腳本。
(function () {
    'use strict';

    // ---- 佈景主題：跟隨系統，也允許使用者手動切換並記住選擇 -----------------
    var THEME_KEY = 'cip-theme';

    function applyTheme(theme) {
        var root = document.documentElement;
        if (theme === 'light' || theme === 'dark') {
            root.setAttribute('data-theme', theme);
        } else {
            root.removeAttribute('data-theme');
        }
        document.querySelectorAll('[data-cip-theme-toggle]').forEach(function (btn) {
            var isDark = theme === 'dark' ||
                (theme !== 'light' && window.matchMedia('(prefers-color-scheme: dark)').matches);
            btn.setAttribute('aria-pressed', String(isDark));
            btn.title = isDark ? '切換為淺色模式' : '切換為深色模式';
            var icon = btn.querySelector('[data-cip-theme-icon]');
            if (icon) { icon.textContent = isDark ? '☀' : '☾'; }
        });
    }

    function storedTheme() {
        try { return window.localStorage.getItem(THEME_KEY); } catch (e) { return null; }
    }

    function storeTheme(theme) {
        try {
            if (theme) { window.localStorage.setItem(THEME_KEY, theme); }
            else { window.localStorage.removeItem(THEME_KEY); }
        } catch (e) { /* 隱私模式下 localStorage 可能不可用，忽略即可 */ }
    }

    applyTheme(storedTheme());

    document.addEventListener('click', function (e) {
        var toggle = e.target.closest('[data-cip-theme-toggle]');
        if (!toggle) { return; }
        e.preventDefault();
        var isDark = document.documentElement.getAttribute('data-theme') === 'dark' ||
            (!document.documentElement.hasAttribute('data-theme') &&
                window.matchMedia('(prefers-color-scheme: dark)').matches);
        var next = isDark ? 'light' : 'dark';
        storeTheme(next);
        applyTheme(next);
    });

    // ---- 標示目前所在的選單項目 ------------------------------------------
    function markActiveNav() {
        var path = window.location.pathname.replace(/\/+$/, '').toLowerCase();
        if (!path) { return; }
        var segment = path.split('/')[1] || '';
        if (!segment) { return; }

        document.querySelectorAll('.cip-nav a[href]').forEach(function (link) {
            var href = (link.getAttribute('href') || '').replace(/\/+$/, '').toLowerCase();
            if (!href || href === '#' || href.indexOf('/') !== 0) { return; }
            if (href.split('/')[1] === segment) {
                link.classList.add('active');
                link.setAttribute('aria-current', 'page');
                // 同時把所屬的下拉選單標成 active
                var dropdown = link.closest('.dropdown');
                if (dropdown) {
                    var toggle = dropdown.querySelector('.dropdown-toggle');
                    if (toggle) { toggle.classList.add('active'); }
                }
            }
        });
    }

    // ---- 破壞性操作一律再確認 --------------------------------------------
    // 用 data-cip-confirm="訊息" 標註表單或按鈕即可，不需要各頁自己寫 onclick。
    document.addEventListener('submit', function (e) {
        var form = e.target;
        var message = form.getAttribute && form.getAttribute('data-cip-confirm');
        if (message && !window.confirm(message)) {
            e.preventDefault();
        }
    });

    // ---- 避免重複送出 ----------------------------------------------------
    document.addEventListener('submit', function (e) {
        var form = e.target;
        if (!form || form.getAttribute('data-cip-allow-resubmit') === 'true') { return; }
        if (form.dataset.cipSubmitting === '1') { e.preventDefault(); return; }
        form.dataset.cipSubmitting = '1';
        // 表單驗證失敗時要能再按一次
        window.setTimeout(function () { delete form.dataset.cipSubmitting; }, 4000);
    });

    // ---- 提示訊息自動淡出 ------------------------------------------------
    function autoDismissAlerts() {
        document.querySelectorAll('[data-cip-autodismiss]').forEach(function (el) {
            var delay = parseInt(el.getAttribute('data-cip-autodismiss'), 10) || 6000;
            window.setTimeout(function () {
                el.style.transition = 'opacity .4s ease';
                el.style.opacity = '0';
                window.setTimeout(function () { el.remove(); }, 450);
            }, delay);
        });
    }

    function ready(fn) {
        if (document.readyState !== 'loading') { fn(); }
        else { document.addEventListener('DOMContentLoaded', fn); }
    }

    ready(function () {
        markActiveNav();
        autoDismissAlerts();
        applyTheme(storedTheme());
    });
})();
