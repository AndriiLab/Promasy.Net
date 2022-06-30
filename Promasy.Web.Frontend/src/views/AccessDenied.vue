<template>
    <div class="surface-0 flex align-items-center justify-content-center min-h-screen min-w-screen overflow-hidden">
        <div class="grid justify-content-center p-2 lg:p-0" style="min-width:80%">
            <div class="col-12 mt-5 xl:mt-0 text-center">
                <img src="@/assets/logo.png" alt="Logo" class="mb-5">
            </div>
            <div class="col-12 xl:col-6" style="border-radius:56px; padding:0.3rem; background: linear-gradient(180deg, rgba(247, 149, 48, 0.4) 10%, rgba(247, 149, 48, 0) 30%);">
                <div class="h-full w-full m-0 py-7 px-4" style="border-radius:53px; background: linear-gradient(180deg, var(--surface-50) 38.9%, var(--surface-0));">
                    <div class="grid flex flex-column align-items-center">
                        <div class="flex justify-content-center align-items-center bg-orange-500 border-circle" style="width:3.2rem; height:3.2rem;">
                            <i class="pi pi-fw pi-lock text-2xl text-50"></i>
                        </div>
                        <h1 class="text-900 font-bold text-4xl lg:text-5xl mb-2">{{ t('accessDenied')}}</h1>
                        <span class="text-600 text-center">{{ t('notEnoughPermissions') }}</span>
                        <AssetAccess alt="Access denied" class="mt-5"></AssetAccess>
                        <div class="col-12 mt-5 text-center">
                            <i class="pi pi-fw pi-arrow-left text-blue-500 mr-2" style="vertical-align:center;"></i>
                            <router-link :to="getLastUrl" class="text-blue-500">{{ t('goBack') }}</router-link>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts" setup>
import AssetAccess from '@/assets/images/asset-access.svg'
import { useI18n } from 'vue-i18n';
import { useSessionStore } from '@/store/session';

const { t } = useI18n({ useScope: "local" });
const { getLastUrl } = useSessionStore();
</script>

<i18n locale="en">
{
    "accessDenied": "Access Denied",
    "notEnoughPermissions": "You do not have the necesary permisions. Please contact admins.",
    "goBack": "Go Back",
}
</i18n>

<i18n locale="uk">
{
    "accessDenied": "Доступ заборонено",
    "notEnoughPermissions": "У вас відсутні необхідні дозволи. Будь ласка, сконтактуйте з адміністратом.",
    "goBack": "Повернутись назад",
}
</i18n>