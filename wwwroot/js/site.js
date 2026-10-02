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
    }, { threshold: 0.1 });
    revealItems.forEach(function (el) { observer.observe(el); });
} else {
    revealItems.forEach(function (el) { el.classList.add('show'); });
}

// Show toast messages
document.querySelectorAll('.toast').forEach(function (el) {
    new bootstrap.Toast(el, { delay: 3500 }).show();
});