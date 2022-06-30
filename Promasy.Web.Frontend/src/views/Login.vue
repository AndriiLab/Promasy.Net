<template>
    <div class="surface-0 flex align-items-center justify-content-center min-h-screen min-w-screen overflow-hidden">
        <div class="grid justify-content-center p-2 lg:p-0" style="min-width:80%">
            <div class="col-12 mt-5 xl:mt-0 text-center">
            </div>
            <div class="col-12 xl:col-6"
                style="border-radius:56px; padding:0.3rem; background: linear-gradient(180deg, var(--primary-color), rgba(33, 150, 243, 0) 30%);">
                <div class="h-full w-full m-0 py-7 px-4"
                    style="border-radius:53px; background: linear-gradient(180deg, var(--surface-50) 38.9%, var(--surface-0));">
                    <div class="text-center mb-5">
                        <img :src="'/src/assets/logo.png'" alt="Promasy logo" height="50" class="mb-3">
                        <div class="text-900 text-3xl font-medium mb-3">{{ t('welcome') }}</div>
                        <span class="text-600 font-medium">{{ t('signInToContinue') }}</span>
                    </div>

                    <Message v-for="err of errors" :severity="'error'" :key="err" :closable="false">{{err}}</Message>
                    <form class="w-full md:w-10 mx-auto">
                        <ErrorWrap :errors="v$.username.$errors" :class="['mb-3']">
                            <label for="username" class="block text-900 text-xl font-medium mb-2">{{ t('username') }}</label>
                            <InputText id="username" v-model.trim="model.username" type="text" autocomplete="username" class="w-full"
                                :placeholder="t('username')" style="padding:1rem;" />
                        </ErrorWrap>

                        <ErrorWrap :errors="v$.password.$errors" :class="['mb-3']">
                            <label for="password" class="block text-900 font-medium text-xl mb-2">{{ t('password') }}</label>
                            <Password id="password" v-model="model.password" :placeholder="t('password')" :toggleMask="true"
                                autocomplete="password" class="w-full" inputClass="w-full"
                                inputStyle="padding:1rem" :feedback="true" 
                                :promptLabel="t('enterPassword')" :weakLabel="t('weakPassword')"
                                :mediumLabel="t('mediumPassword')" :goodLabel="t('goodPassword')"
                                :strongLabel="t('strongPassword')"></Password>
                        </ErrorWrap>

                        <div class="flex align-items-center justify-content-between mb-5">
                            <div class="flex align-items-center">
                                <Checkbox id="rememberme" v-model="model.rememberMe" :binary="true" class="mr-2"></Checkbox>
                                <label for="rememberme">{{ t('rememberMe') }}</label>
                            </div>
                            <a class="font-medium no-underline ml-2 text-right cursor-pointer"
                                style="color: var(--primary-color)">{{ t('forgotPassword') }}</a>
                        </div>
                        <Button :label="t('signIn')" class="w-full p-3 text-xl" @click="submitLogin"></Button>
                    </form>

                    <div class="mt-5">
                        <LanguageSelector :labelClasses="['ml-7']" :selectorClasses="['ml-2']"></LanguageSelector>
                    </div>

                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts" setup>
import { reactive, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { useSessionStore } from '@/store/session';
import useVuelidate from '@vuelidate/core';
import { required } from '@/i18n/validators';
import ErrorWrap from "../components/ErrorWrap.vue";
import { ErrorApiResponse } from "@/utils/fetch-utils";
import LanguageSelector from "@/components/LanguageSelector.vue";
import LocalStore, { keys } from "../services/local-store";
import { Router } from "@/router";

const { t } = useI18n({ useScope: "local" });
const sessionStore = useSessionStore();
const model = reactive({
    username: '',
    password: '',
    rememberMe: !!LocalStore.get(keys.allowStore),
});
const errors : string[] = reactive([]);
const rules = computed(() => {
    return {
        username: { required },
        password: { required }
    }
});
const v$ = useVuelidate(rules, model, { $lazy: true });

async function submitLogin() {
    const isFormCorrect = await v$.value.$validate();
    if (!isFormCorrect) return;
    try {
        await sessionStore.loginAsync(model.username, model.password, model.rememberMe);
        Router.push(sessionStore.getLastUrl);
    } catch (err : any) {
        const apiErr = err as ErrorApiResponse;
        errors.splice(0);
        for (const errArray of Object.values(apiErr.errors)) {
            errors.push(...errArray);
        }
    }
}
</script>


<style scoped>
.pi-eye {
    transform: scale(1.6);
    margin-right: 1rem;
}

.pi-eye-slash {
    transform: scale(1.6);
    margin-right: 1rem;
}
</style>

<i18n locale="en">
{
    "welcome": "Welcome to Promasy",
    "signInToContinue": "Sign in to continue",
    "username": "Username",
    "password": "Password",
    "signIn": "Sign In",
    "rememberMe": "Remember me",
    "forgotPassword": "Forgot password?",
    "enterPassword": "Enter a password",
    "weakPassword": "Weak",
    "mediumPassword": "Medium",
    "goodPassword": "Good",
    "strongPassword": "Strong",
}
</i18n>

<i18n locale="uk">
{
    "welcome": "Вітаємо у Promasy",
    "signInToContinue": "Увійдіть, щоб продовжити роботу",
    "username": "Ім'я користувача",
    "password": "Пароль",
    "signIn": "Вхід",
    "rememberMe": "Запам'ятати мене",
    "forgotPassword": "Забули пароль?",
    "enterPassword": "Введіть пароль",
    "weakPassword": "Слабкий",
    "mediumPassword": "Середній",
    "goodPassword": "Хороший",
    "strongPassword": "Сильний",
}
</i18n>