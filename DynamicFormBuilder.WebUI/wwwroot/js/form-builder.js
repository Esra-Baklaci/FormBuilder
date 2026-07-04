(function () {
    'use strict';

    const config = window.formBuilderConfig;
    if (!config) return;

    const FIELD_META = {
        1:  { name: 'Textbox',       icon: 'bi-input-cursor-text',   group: 'basic' },
        2:  { name: 'Textarea',      icon: 'bi-textarea-t',          group: 'basic' },
        3:  { name: 'Number',        icon: 'bi-123',                 group: 'basic' },
        4:  { name: 'Email',         icon: 'bi-envelope',            group: 'basic' },
        5:  { name: 'Phone',         icon: 'bi-telephone',           group: 'basic' },
        6:  { name: 'Date',          icon: 'bi-calendar-date',       group: 'basic' },
        7:  { name: 'Time',          icon: 'bi-clock',               group: 'basic' },
        8:  { name: 'Password',      icon: 'bi-key',                 group: 'basic' },
        9:  { name: 'Hidden Field',  icon: 'bi-eye-slash',           group: 'basic' },
        10: { name: 'Dropdown',      icon: 'bi-menu-button-wide',    group: 'choice' },
        11: { name: 'Radio Button',  icon: 'bi-record-circle',       group: 'choice' },
        12: { name: 'Checkbox',      icon: 'bi-check-square',        group: 'choice' },
        13: { name: 'Multi Select',  icon: 'bi-ui-checks',           group: 'choice' },
        14: { name: 'File Upload',   icon: 'bi-file-earmark-arrow-up', group: 'advanced' },
        15: { name: 'Image Upload',  icon: 'bi-image',               group: 'advanced' },
        16: { name: 'Signature',     icon: 'bi-pen',                 group: 'advanced' },
        17: { name: 'Rating',        icon: 'bi-star',                group: 'advanced' },
        18: { name: 'Slider',        icon: 'bi-sliders',             group: 'advanced' },
        19: { name: 'Heading',       icon: 'bi-type-h1',             group: 'layout' },
        20: { name: 'Paragraph',     icon: 'bi-text-paragraph',      group: 'layout' },
        21: { name: 'Divider',       icon: 'bi-hr',                  group: 'layout' },
        22: { name: 'Spacer',        icon: 'bi-distribute-vertical', group: 'layout' },
        23: { name: 'Image',         icon: 'bi-card-image',          group: 'layout' },
        24: { name: 'HTML Block',    icon: 'bi-code-slash',          group: 'layout' }
    };

    const CHOICE_TYPES = [10, 11, 12, 13];
    const TEXT_TYPES = [1, 2, 4, 5, 8];
    const NUMERIC_TYPES = [3, 17, 18];
    const LAYOUT_TYPES = [19, 20, 21, 22, 23, 24];
    const NO_PLACEHOLDER = [9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24];
    const NO_REQUIRED = [19, 20, 21, 22, 23, 24];

    const OPERATORS = [
        { value: 1, label: 'eşittir' },
        { value: 2, label: 'eşit değil' },
        { value: 3, label: 'içerir' },
        { value: 4, label: 'büyüktür' },
        { value: 5, label: 'küçüktür' }
    ];

    const ACTIONS = [
        { value: 1, label: 'göster' },
        { value: 2, label: 'gizle' }
    ];

    const WIDTH_PRESETS = ['25%', '33%', '50%', '75%', '100%'];

    let fields = [];
    let conditionalLogics = [];
    let selectedClientId = null;
    let libraryDragging = false;

    const canvas = document.getElementById('formCanvas');
    const placeholder = document.getElementById('canvasPlaceholder');
    const propertiesPanel = document.getElementById('propertiesPanel');
    const logicList = document.getElementById('logicList');

    function fieldName(type) {
        return FIELD_META[type]?.name || 'Field';
    }

    function generateClientId() {
        return 'fld_' + Math.random().toString(36).substring(2, 14);
    }

    function createDefaultField(fieldType) {
        const type = parseInt(fieldType, 10);
        const name = fieldName(type);
        const existingCount = fields.filter(f => f.fieldType === type).length;
        const label = existingCount > 0 ? `${name} ${existingCount + 1}` : name;

        return {
            id: null,
            clientId: generateClientId(),
            fieldType: type,
            label: label,
            placeholder: '',
            defaultValue: type === 18 ? '50' : (type === 17 ? '3' : ''),
            helpText: '',
            isRequired: false,
            isReadOnly: false,
            isHidden: false,
            options: CHOICE_TYPES.includes(type) ? 'Seçenek 1\nSeçenek 2\nSeçenek 3' : '',
            minLength: null,
            maxLength: null,
            minValue: type === 17 ? 1 : (type === 18 ? 0 : null),
            maxValue: type === 17 ? 5 : (type === 18 ? 100 : null),
            regexPattern: '',
            width: '100%',
            cssClass: '',
            sortOrder: fields.length
        };
    }

    function normalizeField(field) {
        return {
            id: field.id ?? null,
            clientId: field.clientId || generateClientId(),
            fieldType: field.fieldType,
            label: field.label || fieldName(field.fieldType),
            placeholder: field.placeholder || '',
            defaultValue: field.defaultValue || '',
            helpText: field.helpText || '',
            isRequired: !!field.isRequired,
            isReadOnly: !!field.isReadOnly,
            isHidden: !!field.isHidden,
            options: field.options || '',
            minLength: field.minLength,
            maxLength: field.maxLength,
            minValue: field.minValue,
            maxValue: field.maxValue,
            regexPattern: field.regexPattern || '',
            width: field.width || '100%',
            cssClass: field.cssClass || '',
            sortOrder: field.sortOrder ?? 0
        };
    }

    function recalculateSortOrder() {
        const elements = canvas.querySelectorAll('.canvas-field');
        elements.forEach((el, index) => {
            const clientId = el.dataset.clientId;
            const field = fields.find(f => f.clientId === clientId);
            if (field) field.sortOrder = index;
        });
        fields.sort((a, b) => a.sortOrder - b.sortOrder);
    }

    function togglePlaceholder() {
        placeholder.style.display = fields.length === 0 ? 'block' : 'none';
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    function escapeAttr(text) {
        return String(text ?? '').replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/</g, '&lt;');
    }

    function widthToGridSpan(width) {
        const map = { '100%': 12, '75%': 9, '50%': 6, '33%': 4, '25%': 3 };
        return map[width] || 12;
    }

    function gridSpanToWidth(span) {
        const map = { 12: '100%', 9: '75%', 6: '50%', 4: '33%', 3: '25%' };
        return map[span] || '100%';
    }

    function snapGridSpan(span) {
        const targets = [3, 4, 6, 9, 12];
        return targets.reduce((a, b) => Math.abs(b - span) < Math.abs(a - span) ? b : a);
    }

    function applyFieldLayout(element, width) {
        const span = widthToGridSpan(width || '100%');
        element.style.gridColumn = `span ${span}`;
        element.dataset.gridSpan = String(span);
    }

    function getMockPreview(field) {
        if (LAYOUT_TYPES.includes(field.fieldType)) {
            if (field.fieldType === 21) return '────────────';
            if (field.fieldType === 22) return `↕ ${field.defaultValue || 24}px boşluk`;
            return '';
        }
        if (NO_PLACEHOLDER.includes(field.fieldType)) return field.helpText || '—';
        return field.placeholder || `${fieldName(field.fieldType)} önizleme`;
    }

    function setFieldWidth(clientId, width) {
        const field = fields.find(f => f.clientId === clientId);
        if (!field) return;
        field.width = width;
        renderCanvas();
        if (selectedClientId === clientId) renderProperties();
    }

    function initResizeHandle(handle, field) {
        handle.addEventListener('mousedown', (e) => {
            e.preventDefault();
            e.stopPropagation();
            const canvasRect = canvas.getBoundingClientRect();
            const padding = 24;

            function onMove(ev) {
                const relativeX = ev.clientX - canvasRect.left - padding;
                const available = canvasRect.width - padding * 2;
                const colWidth = available / 12;
                const span = snapGridSpan(Math.max(3, Math.min(12, Math.round(relativeX / colWidth))));
                field.width = gridSpanToWidth(span);
                const el = canvas.querySelector(`[data-client-id="${field.clientId}"]`);
                if (el) {
                    applyFieldLayout(el, field.width);
                    const badge = el.querySelector('.field-width-badge');
                    if (badge) badge.textContent = field.width;
                }
            }

            function onUp() {
                document.removeEventListener('mousemove', onMove);
                document.removeEventListener('mouseup', onUp);
                renderCanvas();
                renderProperties();
            }

            document.addEventListener('mousemove', onMove);
            document.addEventListener('mouseup', onUp);
        });
    }

    function showSaveModal(success, message) {
        const modalEl = document.getElementById('saveModal');
        const iconEl = document.getElementById('saveModalIcon');
        const titleEl = document.getElementById('saveModalTitle');
        const messageEl = document.getElementById('saveModalMessage');
        const btnEl = document.getElementById('saveModalBtn');

        if (!modalEl) return;

        if (success) {
            iconEl.className = 'bi bi-check-circle-fill text-success';
            iconEl.style.fontSize = '3rem';
            titleEl.textContent = 'Kaydedildi';
            messageEl.textContent = message || 'Form başarıyla kaydedildi.';
            btnEl.className = 'btn btn-fb btn-fb-primary';
            btnEl.textContent = 'Form Listesine Git';
            btnEl.onclick = () => { window.location.href = config.formsListUrl; };
        } else {
            iconEl.className = 'bi bi-x-circle-fill text-danger';
            iconEl.style.fontSize = '3rem';
            titleEl.textContent = 'Hata';
            messageEl.textContent = message || 'Kaydetme sırasında bir hata oluştu.';
            btnEl.className = 'btn btn-fb btn-fb-outline';
            btnEl.textContent = 'Tamam';
            btnEl.onclick = () => bootstrap.Modal.getInstance(modalEl).hide();
        }

        bootstrap.Modal.getOrCreateInstance(modalEl).show();
    }

    function renderCanvasField(field) {
        const div = document.createElement('div');
        div.className = 'canvas-field' + (selectedClientId === field.clientId ? ' selected' : '');
        div.dataset.clientId = field.clientId;
        div.dataset.fieldType = field.fieldType;

        const meta = FIELD_META[field.fieldType];
        const metaParts = [];
        if (!NO_REQUIRED.includes(field.fieldType)) metaParts.push(field.isRequired ? 'Zorunlu' : 'Opsiyonel');
        if (field.isHidden) metaParts.push('Gizli');
        if (field.isReadOnly) metaParts.push('Salt okunur');
        if (CHOICE_TYPES.includes(field.fieldType) && field.options) {
            const optCount = field.options.split('\n').filter(o => o.trim()).length;
            metaParts.push(`${optCount} seçenek`);
        }

        const mockPreview = getMockPreview(field);

        div.innerHTML = `
            <div class="field-actions">
                <button type="button" class="btn-soft btn-soft-duplicate btn-duplicate" title="Kopyala"><i class="bi bi-copy"></i></button>
                <button type="button" class="btn-soft btn-soft-delete btn-delete" title="Sil"><i class="bi bi-trash"></i></button>
            </div>
            <div class="resize-handle" title="Genişliği sürükleyerek değiştir"></div>
            <div class="field-type-badge"><i class="bi ${meta?.icon || 'bi-ui-radios'} me-1"></i>${fieldName(field.fieldType)}</div>
            <div class="field-preview-label">${escapeHtml(field.label)}</div>
            <div class="field-meta">${metaParts.join(' · ') || '—'}</div>
            ${mockPreview ? `<div class="field-preview-mock">${escapeHtml(mockPreview)}</div>` : ''}
            <span class="field-width-badge">${field.width || '100%'}</span>
        `;

        applyFieldLayout(div, field.width);

        div.addEventListener('click', (e) => {
            if (e.target.closest('.btn-delete') || e.target.closest('.btn-duplicate') || e.target.closest('.resize-handle')) return;
            selectField(field.clientId);
        });

        div.querySelector('.btn-delete').addEventListener('click', (e) => {
            e.stopPropagation();
            deleteField(field.clientId);
        });

        div.querySelector('.btn-duplicate').addEventListener('click', (e) => {
            e.stopPropagation();
            duplicateField(field.clientId);
        });

        initResizeHandle(div.querySelector('.resize-handle'), field);

        return div;
    }

    function renderCanvas() {
        canvas.querySelectorAll('.canvas-field').forEach(el => el.remove());
        fields.sort((a, b) => a.sortOrder - b.sortOrder).forEach(field => {
            canvas.appendChild(renderCanvasField(field));
        });
        togglePlaceholder();
    }

    function selectField(clientId) {
        selectedClientId = clientId;
        renderCanvas();
        renderProperties();
    }

    function deleteField(clientId) {
        fields = fields.filter(f => f.clientId !== clientId);
        conditionalLogics = conditionalLogics.filter(l =>
            l.sourceFieldClientId !== clientId && l.targetFieldClientId !== clientId);
        if (selectedClientId === clientId) selectedClientId = null;
        recalculateSortOrder();
        renderCanvas();
        renderProperties();
        renderLogics();
    }

    function duplicateField(clientId) {
        const source = fields.find(f => f.clientId === clientId);
        if (!source) return;

        const copy = normalizeField({ ...source, id: null, clientId: generateClientId() });
        copy.label = source.label + ' (kopya)';
        copy.sortOrder = fields.length;
        fields.push(copy);
        renderCanvas();
        selectField(copy.clientId);
    }

    function addField(fieldType) {
        const field = createDefaultField(fieldType);
        field.sortOrder = fields.length;
        fields.push(field);
        renderCanvas();
        selectField(field.clientId);
        clearLibraryFocus();
    }

    function clearLibraryFocus() {
        document.querySelectorAll('.library-item').forEach(item => {
            item.classList.remove('sortable-chosen', 'sortable-ghost', 'sortable-drag');
        });
        if (document.activeElement && document.activeElement !== document.body) {
            document.activeElement.blur();
        }
    }

    function propSection(title, content) {
        return `<div class="prop-section"><div class="prop-section-title">${title}</div>${content}</div>`;
    }

    function renderProperties() {
        const field = fields.find(f => f.clientId === selectedClientId);
        if (!field) {
            propertiesPanel.innerHTML = '<p class="text-muted small">Düzenlemek için bir alan seçin veya soldan yeni alan ekleyin.</p>';
            return;
        }

        const type = field.fieldType;
        const isLayout = LAYOUT_TYPES.includes(type);
        const isChoice = CHOICE_TYPES.includes(type);
        const isText = TEXT_TYPES.includes(type);
        const isNumeric = NUMERIC_TYPES.includes(type);
        const meta = FIELD_META[type];

        let html = `<div class="mb-2"><span class="badge rounded-pill" style="background:#eef2ff;color:#6b8cff;font-weight:500;"><i class="bi ${meta?.icon} me-1"></i>${fieldName(type)}</span></div>`;

        // Genel
        let general = `
            <div class="mb-2">
                <label class="form-label">${type === 19 ? 'Başlık Metni' : type === 20 ? 'Paragraf Metni' : 'Etiket (Label)'}</label>
                <input type="text" class="form-control form-control-sm" data-prop="label" value="${escapeAttr(field.label)}">
            </div>`;

        if (!NO_PLACEHOLDER.includes(type)) {
            general += `
            <div class="mb-2">
                <label class="form-label">Placeholder</label>
                <input type="text" class="form-control form-control-sm" data-prop="placeholder" value="${escapeAttr(field.placeholder)}">
            </div>`;
        }

        if (type === 23) {
            general += `
            <div class="mb-2">
                <label class="form-label">Görsel URL</label>
                <input type="text" class="form-control form-control-sm" data-prop="defaultValue" value="${escapeAttr(field.defaultValue)}" placeholder="https://...">
            </div>`;
        } else if (type === 24) {
            general += `
            <div class="mb-2">
                <label class="form-label">HTML İçerik</label>
                <textarea class="form-control form-control-sm" data-prop="defaultValue" rows="4">${escapeAttr(field.defaultValue)}</textarea>
            </div>`;
        } else if (type === 22) {
            general += `
            <div class="mb-2">
                <label class="form-label">Boşluk Yüksekliği (px)</label>
                <input type="number" class="form-control form-control-sm" data-prop="defaultValue" value="${escapeAttr(field.defaultValue || '24')}" min="8" max="200">
            </div>`;
        } else if (!isLayout) {
            general += `
            <div class="mb-2">
                <label class="form-label">Varsayılan Değer</label>
                <input type="text" class="form-control form-control-sm" data-prop="defaultValue" value="${escapeAttr(field.defaultValue)}">
            </div>`;
        }

        if (!isLayout || type === 19 || type === 20) {
            general += `
            <div class="mb-2">
                <label class="form-label">Yardım Metni</label>
                <input type="text" class="form-control form-control-sm" data-prop="helpText" value="${escapeAttr(field.helpText)}">
            </div>`;
        }

        html += propSection('Genel', general);

        // Seçenekler
        if (isChoice) {
            html += propSection('Seçenekler', `
                <div class="mb-2">
                    <label class="form-label">Seçenekler (her satır bir seçenek)</label>
                    <textarea class="form-control form-control-sm" data-prop="options" rows="4">${escapeAttr(field.options)}</textarea>
                </div>`);
        }

        // Doğrulama
        if (!isLayout) {
            let validation = '';
            if (isText || type === 2) {
                validation += `
                <div class="row g-2 mb-2">
                    <div class="col-6">
                        <label class="form-label">Min Uzunluk</label>
                        <input type="number" class="form-control form-control-sm" data-prop="minLength" value="${field.minLength ?? ''}">
                    </div>
                    <div class="col-6">
                        <label class="form-label">Max Uzunluk</label>
                        <input type="number" class="form-control form-control-sm" data-prop="maxLength" value="${field.maxLength ?? ''}">
                    </div>
                </div>`;
            }
            if (isNumeric) {
                validation += `
                <div class="row g-2 mb-2">
                    <div class="col-6">
                        <label class="form-label">Min Değer</label>
                        <input type="number" class="form-control form-control-sm" data-prop="minValue" value="${field.minValue ?? ''}">
                    </div>
                    <div class="col-6">
                        <label class="form-label">Max Değer</label>
                        <input type="number" class="form-control form-control-sm" data-prop="maxValue" value="${field.maxValue ?? ''}">
                    </div>
                </div>`;
            }
            if (isText || type === 2 || type === 4 || type === 5) {
                validation += `
                <div class="mb-2">
                    <label class="form-label">Regex Deseni</label>
                    <input type="text" class="form-control form-control-sm" data-prop="regexPattern" value="${escapeAttr(field.regexPattern)}" placeholder="örn: ^[A-Za-z]+$">
                </div>`;
            }
            if (!NO_REQUIRED.includes(type)) {
                validation += `
                <div class="form-check mb-1">
                    <input class="form-check-input" type="checkbox" data-prop="isRequired" ${field.isRequired ? 'checked' : ''}>
                    <label class="form-check-label">Zorunlu alan</label>
                </div>`;
            }
            validation += `
                <div class="form-check mb-1">
                    <input class="form-check-input" type="checkbox" data-prop="isReadOnly" ${field.isReadOnly ? 'checked' : ''}>
                    <label class="form-check-label">Salt okunur</label>
                </div>
                <div class="form-check mb-1">
                    <input class="form-check-input" type="checkbox" data-prop="isHidden" ${field.isHidden ? 'checked' : ''}>
                    <label class="form-check-label">Gizli</label>
                </div>`;
            html += propSection('Doğrulama & Davranış', validation);
        }

        // Görünüm
        const widthBtns = WIDTH_PRESETS.map(w =>
            `<button type="button" class="${field.width === w ? 'active' : ''}" data-width-btn="${w}">${w}</button>`
        ).join('');

        html += propSection('Görünüm', `
            <div class="mb-2">
                <label class="form-label">Genişlik</label>
                <div class="width-quick-btns mb-2">${widthBtns}</div>
                <select class="form-select form-select-sm" data-prop="width">
                    <option value="100%" ${field.width === '100%' ? 'selected' : ''}>Tam genişlik (100%)</option>
                    <option value="75%" ${field.width === '75%' ? 'selected' : ''}>75%</option>
                    <option value="50%" ${field.width === '50%' ? 'selected' : ''}>Yarım (50%)</option>
                    <option value="33%" ${field.width === '33%' ? 'selected' : ''}>Üçte bir (33%)</option>
                    <option value="25%" ${field.width === '25%' ? 'selected' : ''}>Çeyrek (25%)</option>
                </select>
                <div class="form-text">Sağ kenardan sürükleyerek genişlik ayarlayın. Yan yana koymak için alanı sürükleyip yanına bırakın (örn. iki %50).</div>
            </div>
            <div class="mb-2">
                <label class="form-label">CSS Sınıfı</label>
                <input type="text" class="form-control form-control-sm" data-prop="cssClass" value="${escapeAttr(field.cssClass)}" placeholder="örn: my-custom-class">
            </div>`);

        propertiesPanel.innerHTML = html;

        propertiesPanel.querySelectorAll('[data-width-btn]').forEach(btn => {
            btn.addEventListener('click', () => {
                setFieldWidth(field.clientId, btn.dataset.widthBtn);
            });
        });

        propertiesPanel.querySelectorAll('[data-prop]').forEach(input => {
            const prop = input.dataset.prop;
            const eventType = input.type === 'checkbox' || input.tagName === 'SELECT' ? 'change' : 'input';
            input.addEventListener(eventType, () => {
                if (input.type === 'checkbox') {
                    field[prop] = input.checked;
                } else if (input.type === 'number') {
                    field[prop] = input.value === '' ? null : Number(input.value);
                } else {
                    field[prop] = input.value;
                }
                renderCanvas();
                if (prop === 'width') renderProperties();
            });
        });
    }

    function renderLogics() {
        logicList.innerHTML = '';
        conditionalLogics.forEach((logic, index) => {
            const div = document.createElement('div');
            div.className = 'logic-item';
            const sourceOptions = fields.map(f => `<option value="${f.clientId}" ${f.clientId === logic.sourceFieldClientId ? 'selected' : ''}>${escapeHtml(f.label)}</option>`).join('');
            const targetOptions = fields.map(f => `<option value="${f.clientId}" ${f.clientId === logic.targetFieldClientId ? 'selected' : ''}>${escapeHtml(f.label)}</option>`).join('');
            const opOptions = OPERATORS.map(o => `<option value="${o.value}" ${logic.operator === o.value ? 'selected' : ''}>${o.label}</option>`).join('');
            const actOptions = ACTIONS.map(a => `<option value="${a.value}" ${logic.actionType === a.value ? 'selected' : ''}>${a.label}</option>`).join('');

            div.innerHTML = `
                <div class="mb-1">
                    <label class="form-label">Kaynak alan</label>
                    <select class="form-select form-select-sm" data-logic="${index}" data-key="sourceFieldClientId">${sourceOptions}</select>
                </div>
                <div class="mb-1">
                    <label class="form-label">Koşul</label>
                    <select class="form-select form-select-sm" data-logic="${index}" data-key="operator">${opOptions}</select>
                </div>
                <div class="mb-1">
                    <label class="form-label">Değer</label>
                    <input type="text" class="form-control form-control-sm" data-logic="${index}" data-key="value" value="${escapeAttr(logic.value)}" placeholder="Değer">
                </div>
                <div class="mb-1">
                    <label class="form-label">Hedef alan</label>
                    <select class="form-select form-select-sm" data-logic="${index}" data-key="targetFieldClientId">${targetOptions}</select>
                </div>
                <div class="d-flex gap-1 align-items-end">
                    <div class="flex-grow-1">
                        <label class="form-label">Eylem</label>
                        <select class="form-select form-select-sm" data-logic="${index}" data-key="actionType">${actOptions}</select>
                    </div>
                    <button type="button" class="btn-soft btn-soft-delete" data-remove-logic="${index}"><i class="bi bi-trash"></i></button>
                </div>
            `;
            logicList.appendChild(div);
        });

        logicList.querySelectorAll('[data-logic]').forEach(el => {
            el.addEventListener('change', () => {
                const idx = parseInt(el.dataset.logic, 10);
                const key = el.dataset.key;
                conditionalLogics[idx][key] = el.tagName === 'SELECT' && key !== 'value' ? parseInt(el.value, 10) : el.value;
            });
            el.addEventListener('input', () => {
                const idx = parseInt(el.dataset.logic, 10);
                conditionalLogics[idx][el.dataset.key] = el.value;
            });
        });

        logicList.querySelectorAll('[data-remove-logic]').forEach(btn => {
            btn.addEventListener('click', () => {
                conditionalLogics.splice(parseInt(btn.dataset.removeLogic, 10), 1);
                renderLogics();
            });
        });
    }

    function initLibraryInteractions() {
        document.querySelectorAll('.library-item').forEach(item => {
            const fieldType = item.dataset.fieldType;

            item.addEventListener('click', (e) => {
                if (e.target.closest('.lib-add')) return;
                if (libraryDragging) return;
                addField(fieldType);
            });

            const addBtn = item.querySelector('.lib-add');
            if (addBtn) {
                addBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    if (libraryDragging) return;
                    addField(fieldType);
                });
                addBtn.addEventListener('mousedown', (e) => {
                    e.stopPropagation();
                });
            }
        });

        const searchInput = document.getElementById('fieldSearch');
        if (searchInput) {
            searchInput.addEventListener('input', () => {
                const query = searchInput.value.toLowerCase().trim();
                document.querySelectorAll('.library-item').forEach(item => {
                    const label = item.querySelector('.lib-label')?.textContent.toLowerCase() || '';
                    item.classList.toggle('hidden-by-search', query.length > 0 && !label.includes(query));
                });
            });
        }
    }

    function initSortable() {
        new Sortable(canvas, {
            group: { name: 'form-fields', pull: false, put: true },
            animation: 200,
            ghostClass: 'sortable-ghost',
            draggable: '.canvas-field',
            swapThreshold: 0.65,
            invertSwap: true,
            onAdd: function (evt) {
                const item = evt.item;
                const fieldType = item.dataset.fieldType;
                item.remove();
                if (fieldType) addField(fieldType);
                recalculateSortOrder();
            },
            onEnd: function () {
                recalculateSortOrder();
                renderCanvas();
            }
        });

        document.querySelectorAll('.field-library').forEach(lib => {
            new Sortable(lib, {
                group: { name: 'form-fields', pull: 'clone', put: false },
                sort: false,
                animation: 200,
                draggable: '.library-item',
                filter: '.lib-add',
                preventOnFilter: true,
                delay: 150,
                delayOnTouchOnly: false,
                onStart: function () {
                    libraryDragging = true;
                    document.body.classList.add('library-dragging');
                },
                onEnd: function () {
                    libraryDragging = false;
                    document.body.classList.remove('library-dragging');
                    clearLibraryFocus();
                    document.querySelectorAll('.sortable-fallback').forEach(el => el.remove());
                },
                onClone: function (evt) {
                    const clone = evt.clone;
                    clone.dataset.fieldType = evt.item.dataset.fieldType;
                    clone.classList.remove('library-item');
                    clone.classList.add('canvas-field');
                    clone.style.opacity = '0.85';
                    clone.querySelectorAll('.lib-add').forEach(el => el.remove());
                }
            });
        });
    }

    async function saveForm() {
        recalculateSortOrder();

        const payload = {
            formId: config.formId,
            fields: fields.map(f => ({ ...f, fieldType: parseInt(f.fieldType, 10) })),
            conditionalLogics: conditionalLogics.map(l => ({
                ...l,
                operator: parseInt(l.operator, 10),
                actionType: parseInt(l.actionType, 10)
            }))
        };

        const btn = document.getElementById('btnSave');
        btn.disabled = true;
        btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Kaydediliyor...';

        try {
            const response = await fetch(config.saveUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });
            const result = await response.json();
            if (result.success) {
                showSaveModal(true, 'Form alanları başarıyla kaydedildi.');
            } else {
                showSaveModal(false, result.message || 'Kaydetme başarısız.');
            }
        } catch (err) {
            showSaveModal(false, 'Kaydetme hatası: ' + err.message);
        } finally {
            btn.disabled = false;
            btn.innerHTML = '<i class="bi bi-save me-1"></i>Kaydet';
        }
    }

    function initTemplates() {
        document.querySelectorAll('.template-item').forEach(btn => {
            btn.addEventListener('click', () => {
                applyTemplate(btn.dataset.templateId, false);
            });
        });
    }

    function applyTemplate(templateId, silent) {
        const templates = window.FORM_TEMPLATES || {};
        const tpl = templates[templateId];
        if (!tpl) return;

        if (!silent && fields.length > 0) {
            if (!confirm(`"${tpl.name}" şablonu uygulansın mı? Mevcut alanlar silinecek.`)) return;
        }

        conditionalLogics = [];
        fields = tpl.fields.map((f, index) => normalizeField({
            ...f,
            clientId: generateClientId(),
            sortOrder: index
        }));

        selectedClientId = null;
        renderCanvas();
        renderProperties();
        renderLogics();
        clearLibraryFocus();
    }

    function init() {
        if (config.initialData) {
            fields = (config.initialData.fields || []).map(normalizeField);
            conditionalLogics = (config.initialData.conditionalLogics || []).map(l => ({
                id: l.id ?? null,
                sourceFieldClientId: l.sourceFieldClientId,
                operator: l.operator,
                value: l.value || '',
                targetFieldClientId: l.targetFieldClientId,
                actionType: l.actionType
            }));
        }

        renderCanvas();
        renderLogics();
        initLibraryInteractions();
        initSortable();
        initTemplates();

        if (config.templateFromUrl && fields.length === 0) {
            applyTemplate(config.templateFromUrl, true);
        }

        document.getElementById('btnSave').addEventListener('click', saveForm);
        document.getElementById('btnAddLogic').addEventListener('click', () => {
            if (fields.length < 2) {
                alert('Koşullu mantık için en az 2 alan gerekir.');
                return;
            }
            conditionalLogics.push({
                sourceFieldClientId: fields[0].clientId,
                operator: 1,
                value: '',
                targetFieldClientId: fields[1].clientId,
                actionType: 1
            });
            renderLogics();
        });
    }

    init();
})();
