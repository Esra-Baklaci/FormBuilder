(function () {
    'use strict';

    const logics = window.conditionalLogics || [];
    if (!logics.length) return;

    const OPERATORS = {
        1: (a, b) => String(a).toLowerCase() === String(b).toLowerCase(),
        2: (a, b) => String(a).toLowerCase() !== String(b).toLowerCase(),
        3: (a, b) => String(a).toLowerCase().includes(String(b).toLowerCase()),
        4: (a, b) => parseFloat(a) > parseFloat(b),
        5: (a, b) => parseFloat(a) < parseFloat(b)
    };

    function getFieldValue(clientId) {
        const wrapper = document.querySelector(`[data-field-id="${clientId}"]`);
        if (!wrapper) return '';

        const input = wrapper.querySelector('input, select, textarea');
        if (!input) return '';

        if (input.type === 'checkbox') {
            const checked = wrapper.querySelectorAll('input[type="checkbox"]:checked');
            return Array.from(checked).map(c => c.value).join(',');
        }

        if (input.type === 'radio') {
            const selected = wrapper.querySelector('input[type="radio"]:checked');
            return selected ? selected.value : '';
        }

        if (input.tagName === 'SELECT' && input.multiple) {
            return Array.from(input.selectedOptions).map(o => o.value).join(',');
        }

        return input.value;
    }

    function setFieldVisibility(clientId, visible) {
        const wrapper = document.querySelector(`[data-field-id="${clientId}"]`);
        if (!wrapper) return;
        wrapper.classList.toggle('d-none', !visible);
    }

    function evaluateLogics() {
        const targetStates = {};

        logics.forEach(logic => {
            const sourceValue = getFieldValue(logic.sourceFieldClientId);
            const compareFn = OPERATORS[logic.operator];
            const matches = compareFn ? compareFn(sourceValue, logic.value) : false;

            if (!targetStates.hasOwnProperty(logic.targetFieldClientId)) {
                targetStates[logic.targetFieldClientId] = null;
            }

            if (matches) {
                targetStates[logic.targetFieldClientId] = logic.actionType === 1;
            }
        });

        Object.keys(targetStates).forEach(clientId => {
            if (targetStates[clientId] !== null) {
                setFieldVisibility(clientId, targetStates[clientId]);
            }
        });

        logics.forEach(logic => {
            if (!targetStates.hasOwnProperty(logic.targetFieldClientId) || targetStates[logic.targetFieldClientId] === null) {
                const defaultVisible = logic.actionType === 2;
                setFieldVisibility(logic.targetFieldClientId, defaultVisible);
            }
        });
    }

    function bindEvents() {
        document.querySelectorAll('[data-field-id]').forEach(wrapper => {
            wrapper.querySelectorAll('input, select, textarea').forEach(input => {
                input.addEventListener('change', evaluateLogics);
                input.addEventListener('input', evaluateLogics);
            });
        });
    }

    document.addEventListener('DOMContentLoaded', () => {
        bindEvents();
        evaluateLogics();
    });
})();
