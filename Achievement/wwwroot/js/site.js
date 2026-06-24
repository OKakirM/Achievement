// Campo de tags com pesquisa (auto-inicializa qualquer .tag-input da página).
// Dropdown próprio: filtra as opções da BD por correspondência parcial enquanto
// escreves. Cada tag tem um <input type="hidden"> que o form submete (ex.: SelectedGenreIds).
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.tag-input').forEach(initTagInput);
    document.querySelectorAll('input[type=file][data-preview]').forEach(initImagePreview);
    document.querySelectorAll('.review-text').forEach(initReviewClamp);
    document.querySelectorAll('form').forEach(initSubmitLoading);
});

// Spinner no botão de submeter enquanto o form é enviado.
// setTimeout(0) corre após o evento completar: se a validação cancelou
// (defaultPrevented), não bloqueamos o botão. O payload já foi serializado
// no submit síncrono, por isso desativar depois não afeta o envio.
function initSubmitLoading(form) {
    form.addEventListener('submit', function (e) {
        const btn = form.querySelector('button[type=submit], input[type=submit]');
        if (!btn) return;
        setTimeout(function () {
            if (e.defaultPrevented) return;
            btn.disabled = true;
            btn.classList.add('is-loading');
        }, 0);
    });
}

// Corta reviews longas a algumas linhas com gradiente + botão "Ler mais".
// Só adiciona o botão quando o texto realmente transborda o clamp.
function initReviewClamp(el) {
    el.classList.add('clamped');
    if (el.scrollHeight <= el.clientHeight) {
        el.classList.remove('clamped');
        return;
    }
    const btn = document.createElement('button');
    btn.type = 'button';
    btn.className = 'review-more';
    btn.textContent = 'Ler mais';
    btn.addEventListener('click', function () {
        const open = el.classList.toggle('clamped');
        btn.textContent = open ? 'Ler mais' : 'Ler menos';
    });
    el.after(btn);
}

// Pré-visualiza a imagem escolhida antes de submeter.
// data-preview aponta para o wrapper .image-preview-wrap (com <img> e o × de limpar).
function initImagePreview(input) {
    const wrap = document.getElementById(input.dataset.preview);
    if (!wrap) return;
    const img = wrap.querySelector('img');
    const clearBtn = wrap.querySelector('.image-preview-x');

    input.addEventListener('change', function () {
        const file = input.files && input.files[0];
        if (!file) {
            wrap.hidden = true;
            img.removeAttribute('src');
            return;
        }
        const reader = new FileReader();
        reader.onload = e => { img.src = e.target.result; wrap.hidden = false; };
        reader.readAsDataURL(file);
    });

    if (clearBtn) {
        clearBtn.addEventListener('click', function () {
            input.value = '';
            wrap.hidden = true;
            img.removeAttribute('src');
        });
    }
}

function initTagInput(root) {
    const name = root.dataset.name;
    const chips = root.querySelector('.tag-input-chips');
    const search = root.querySelector('.tag-input-search');
    const menu = root.querySelector('.tag-suggestions');
    const source = document.getElementById(search.dataset.source);
    const options = Array.from(source.options).map(o => ({ id: o.dataset.id, text: o.value }));

    const selectedIds = () =>
        Array.from(chips.querySelectorAll('input[type=hidden]')).map(i => i.value);

    function addChip(id, text) {
        if (selectedIds().includes(id)) return;
        const chip = document.createElement('span');
        chip.className = 'chip';
        chip.innerHTML =
            '<span class="chip-label"></span>' +
            '<button type="button" class="chip-x" aria-label="Remover">×</button>' +
            '<input type="hidden">';
        chip.querySelector('.chip-label').textContent = text;
        const hidden = chip.querySelector('input');
        hidden.name = name;
        hidden.value = id;
        chips.appendChild(chip);
    }

    function hideMenu() {
        menu.hidden = true;
        menu.innerHTML = '';
    }

    function renderMenu() {
        const q = search.value.trim().toLowerCase();
        const selected = selectedIds();
        const matches = options
            .filter(o => !selected.includes(o.id) && o.text.toLowerCase().includes(q))
            .slice(0, 8);

        menu.innerHTML = '';
        if (matches.length === 0) {
            const li = document.createElement('li');
            li.className = 'tag-suggestions-empty';
            li.textContent = 'Sem resultados';
            menu.appendChild(li);
        } else {
            matches.forEach(o => {
                const li = document.createElement('li');
                li.textContent = o.text;
                li.dataset.id = o.id;
                menu.appendChild(li);
            });
        }
        menu.hidden = false;
    }

    search.addEventListener('input', renderMenu);
    search.addEventListener('focus', renderMenu);

    // mousedown (antes do blur) para adicionar sem perder o foco do campo.
    menu.addEventListener('mousedown', function (e) {
        const li = e.target.closest('li[data-id]');
        if (!li) return;
        e.preventDefault();
        addChip(li.dataset.id, li.textContent);
        search.value = '';
        renderMenu();
        search.focus();
    });

    search.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') {
            const first = menu.querySelector('li[data-id]');
            if (first) {
                e.preventDefault();
                addChip(first.dataset.id, first.textContent);
                search.value = '';
                renderMenu();
            }
        } else if (e.key === 'Escape') {
            hideMenu();
        }
    });

    search.addEventListener('blur', function () { setTimeout(hideMenu, 120); });

    // Remover (x) — delegação cobre chips iniciais e os adicionados.
    chips.addEventListener('click', function (e) {
        if (e.target.classList.contains('chip-x')) {
            e.target.closest('.chip').remove();
        }
    });
}
