<template>
    <label for="language" :class="labelClasses">{{ t('language') }}</label>
    <Dropdown id="language" v-model="language" :options="avaliableLanguages" optionLabel="name" optionValue="value"
        :class="selectorClasses">
        <template #value="slotProps">
            <div class="flex align-items-center">
                <span :class="'mr-2 flag flag-' + getFlag(slotProps.value)" style="width:18px; height: 12px" />
                <div>{{ getNameByKey(slotProps.value, avaliableLanguages) }}</div>
            </div>
        </template>
        <template #option="slotProps">
            <div class="flex align-items-center">
                <span :class="'mr-2 flag flag-' + getFlag(slotProps.option.value)" style="width:18px; height: 12px" />
                <div>{{ slotProps.option.name }}</div>
            </div>
        </template>
    </Dropdown>
</template>  

<script lang="ts" setup>
import { ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { avaliableLanguages } from '../i18n';
import { useSessionStore } from '../store/session';

defineProps<{
    labelClasses?: string[],
    selectorClasses?: string[],
}>();

const { t } = useI18n({ useScope: "local" });
const sessionStore = useSessionStore();
const language = ref(sessionStore.locale);

function getFlag(language: string) {
    switch (language) {
        case 'en':
            return 'uk';
        case 'uk':
            return 'ua';
        default:
            break;
    }
}

function getNameByKey(key: string, items: SelectObject<string>[]): string | undefined {
    return items.find(i => i.value === key)?.name;
}

watch(language, (l) => sessionStore.setLanguage(l));
</script>

<i18n locale="en">
{
    "language": "Language",
}
</i18n>

<i18n locale="uk">
{
    "language": "Мова",
}
</i18n>