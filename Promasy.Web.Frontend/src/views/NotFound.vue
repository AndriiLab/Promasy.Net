<template>
    <div class="bg-surface-50 dark:bg-surface-950 flex items-center justify-center min-h-screen min-w-[100vw]">
        <div class="flex flex-col items-center justify-center">
            <div style="border-radius: 56px; padding: 0.3rem; background: linear-gradient(180deg, rgba(33, 150, 243, 0.4) 10%, rgba(33, 150, 243, 0) 30%)">
                <div class="w-full bg-surface-0 dark:bg-surface-900 py-20 px-8 sm:px-20 flex flex-col items-center" style="border-radius: 53px">
                    <div class="gap-4 flex flex-col items-center text-center">
                        <span class="text-blue-500 font-bold text-3xl">404</span>
                        <h1 class="text-surface-900 dark:text-surface-0 font-bold text-3xl lg:text-5xl mb-2">{{ t('youAreLost') }}</h1>
                        <span class="text-muted-color mb-8">{{ t('resourceNotAvailable') }}</span>
                        <div class="col-span-12 mt-8 text-center">
                            <Button as="router-link" :label="t('goBack')" :to="getLastUrl" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts" setup>
import { useI18n } from 'vue-i18n';
import { useSessionStore } from '@/store/session';

const { t } = useI18n({ useScope: "local" });
const { getLastUrl } = useSessionStore();
</script>

<i18n locale="en">
{
    "youAreLost": "Looks like you are lost",
    "resourceNotAvailable": "Requested resource is not available."
}
</i18n>

<i18n locale="uk">
{
    "youAreLost": "Схоже, ви загубились",
    "resourceNotAvailable": "Ресурс, до якого був запит, недоступний."
}
</i18n>

