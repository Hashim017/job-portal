// Show or hide password on every password field
document.querySelectorAll('input[type="password"]').forEach(function (input) {
    var wrap = document.createElement('div');
    wrap.className = 'pw-wrap';
    input.parentNode.insertBefore(wrap, input);
    wrap.appendChild(input);

    var btn = document.createElement('button');
    btn.type = 'button';
    btn.className = 'pw-toggle';
    btn.setAttribute('aria-label', 'Show password');
    btn.innerHTML = '<i class="bi bi-eye"></i>';
    btn.addEventListener('click', function () {
        var show = input.type === 'password';
        input.type = show ? 'text' : 'password';
        btn.innerHTML = show ? '<i class="bi bi-eye-slash"></i>' : '<i class="bi bi-eye"></i>';
        btn.setAttribute('aria-label', show ? 'Hide password' : 'Show password');
    });
    wrap.appendChild(btn);
});

// Count up numbers
document.querySelectorAll('[data-count]').forEach(function (el) {
    var target = parseInt(el.getAttribute('data-count'), 10) || 0;
    if (target === 0) {
        el.textContent = '0';
        return;
    }
    var start = null;
    function step(ts) {
        if (!start) start = ts;
        var p = Math.min((ts - start) / 900, 1);
        el.textContent = Math.floor(p * target).toString();
        if (p < 1) {
            requestAnimationFrame(step);
        } else {
            el.textContent = target.toString();
        }
    }
    requestAnimationFrame(step);
});

// Fade in elements when they scroll into view
var revealItems = document.querySelectorAll('.reveal');
if ('IntersectionObserver' in window) {
    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                entry.target.classList.add('show');
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0, rootMargin: '0px 0px -40px 0px' });
    revealItems.forEach(function (el) { observer.observe(el); });
} else {
    revealItems.forEach(function (el) { el.classList.add('show'); });
}

// Show toast messages
document.querySelectorAll('.toast').forEach(function (el) {
    new bootstrap.Toast(el, { delay: 3500 }).show();
});
// Small toast helper
function showToast(message, type) {
    var container = document.querySelector('.toast-container');
    if (!container) return;
    var el = document.createElement('div');
    el.className = 'toast align-items-center text-bg-' + type + ' border-0';
    el.setAttribute('role', 'alert');
    el.innerHTML = '<div class="d-flex"><div class="toast-body"></div>' +
        '<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button></div>';
    el.querySelector('.toast-body').textContent = message;
    container.appendChild(el);
    el.addEventListener('hidden.bs.toast', function () { el.remove(); });
    new bootstrap.Toast(el, { delay: 2500 }).show();
}

// Save or unsave a job without reloading
document.querySelectorAll('.save-form').forEach(function (form) {
    form.addEventListener('submit', function (e) {
        e.preventDefault();
        var btn = form.querySelector('button');
        btn.disabled = true;

        fetch(form.action, {
            method: 'POST',
            body: new FormData(form),
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
            .then(function (r) {
                if (!r.ok) throw new Error('failed');
                return r.json();
            })
            .then(function (data) {
                btn.classList.toggle('saved', data.saved);

                var icon = btn.querySelector('i');
                if (icon) icon.className = 'bi ' + (data.saved ? 'bi-heart-fill' : 'bi-heart');

                var label = btn.querySelector('.save-label');
                if (label) label.textContent = data.saved ? 'Saved' : 'Save job';

                showToast(data.saved ? 'Job saved.' : 'Removed from saved jobs.', 'success');

                if (!data.saved && location.pathname.toLowerCase().indexOf('/savedjobs') === 0) {
                    var item = form.closest('.reveal') || form.closest('.job-wrap');
                    if (item) {
                        item.style.transition = 'opacity 0.3s';
                        item.style.opacity = '0';
                        setTimeout(function () { item.remove(); }, 300);
                    }
                }
            })
            .catch(function () { form.submit(); })
            .finally(function () { btn.disabled = false; });
    });
});