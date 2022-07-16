<template>
    <div class="surface-0 flex align-items-center justify-content-center min-h-screen min-w-screen overflow-hidden">
        <div class="grid justify-content-center p-2 lg:p-0" style="min-width:80%">
            <div class="col-12 mt-5 xl:mt-0 text-center">
                <img src="@/assets/logo.png" alt="Logo" class="mb-5">
            </div>
            <div class="col-12 xl:col-6" style="border-radius:56px; padding:0.3rem; background: linear-gradient(180deg, rgba(33, 150, 243, 0.4) 10%, rgba(33, 150, 243, 0) 30%);">
                <div class="flex justify-content-center h-full w-full m-0 py-7 px-4" style="border-radius:53px; background: linear-gradient(180deg, var(--surface-50) 38.9%, var(--surface-0));">
                    <div class="grid flex-column align-items-center">
                        <h1 class="text-900 font-bold text-3xl lg:text-5xl mb-2"> {{ t('loggingoutYou') }}</h1>
                        <span class="text-gray-600">{{ t('pleaseWait') }}</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts" setup>
import { onMounted } from "vue";
import { useSessionStore } from '@/store/session';
import { useI18n } from 'vue-i18n';
import { useRouter } from "vue-router";

const Router = useRouter();
const { t } = useI18n({ useScope: "local" });

onMounted(async () => {
    await (useSessionStore()).logoutAsync();
    await Router.push({ name: "Login" });
});
</script>

<i18n locale="en">
{
    "loggingoutYou": "Logging you out...",
    "pleaseWait": "Please, wait a bit",
}
</i18n>

<i18n locale="uk">
{
    "loggingoutYou": "Виходимо з Вашого профілю...",
    "pleaseWait": "Будь ласка, зачекайте",
}
</i18n>