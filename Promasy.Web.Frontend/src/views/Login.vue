<template>
  <div class="bg-surface-50 dark:bg-surface-950 flex items-center justify-center min-h-screen min-w-[100vw] overflow-hidden">
    <div class="flex flex-col items-center justify-center">
      <div style="border-radius: 56px; padding: 0.3rem; background: linear-gradient(180deg, var(--primary-color) 10%, rgba(33, 150, 243, 0) 30%)">
        <div class="w-full bg-surface-0 dark:bg-surface-900 py-20 px-8 sm:px-20" style="border-radius: 53px">
          <div class="text-center mb-8">
            <img :src="'/src/assets/logo.png'" alt="Promasy logo" height="50" class="mb-8 w-16 shrink-0 mx-auto">
            <div class="text-surface-900 dark:text-surface-0 text-3xl font-medium mb-4">{{ t('welcome') }}</div>
            <span class="text-muted-color font-medium">{{ t('signInToContinue') }}</span>
          </div>

          <div>
            <Message v-for="err of externalErrors['']" :severity="'error'" :key="err" :closable="false">{{ err }}</Message>

            <ErrorWrap :errors="v$.username.$errors" :externalErrors="externalErrors['Username']">
              <label for="username" class="block text-surface-900 dark:text-surface-0 text-xl font-medium mb-2">{{ t('username') }}</label>
              <InputText id="username" v-model.trim="model.username" type="text" autocomplete="username" class="w-full md:w-[30rem] mb-8"
                         :placeholder="t('username')" />
            </ErrorWrap>

            <ErrorWrap :errors="v$.password.$errors" :externalErrors="externalErrors['Password']">
              <label for="password" class="block text-surface-900 dark:text-surface-0 font-medium text-xl mb-2">{{ t('password') }}</label>
              <Password id="password" v-model="model.password" :placeholder="t('password')"
                             class="mb-4" fluid :toggleMask="true" :feedback="false" autocomplete="password"></Password>
            </ErrorWrap>

            <div class="flex items-center justify-between mt-2 mb-8 gap-8">
              <div class="flex items-center">
                <Checkbox id="rememberme" v-model="model.rememberMe" binary class="mr-2"></Checkbox>
                <label for="rememberme">{{ t('rememberMe') }}</label>
              </div>
              <span class="font-medium no-underline ml-2 text-right cursor-pointer text-primary">{{ t('forgotPassword') }}</span>
            </div>
            <Button :label="t('signIn')" class="w-full" @click="submitLogin"></Button>

            <div class="mt-8 text-center">
              <label for="language" class="mr-3">{{ t('language') }}</label>
              <LanguageSelector id="language" classes="ml-2"></LanguageSelector>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import processError from "@/utils/error-response-utils";
import { reactive, ref, computed } from "vue";
import { useI18n } from "vue-i18n";
import { useSessionStore } from "@/store/session";
import useVuelidate from "@vuelidate/core";
import { required } from "@/i18n/validators";
import { useRouter } from "vue-router";
import ErrorWrap from "@/components/ErrorWrap.vue";
import { ErrorApiResponse } from "@/utils/fetch-utils";
import LanguageSelector from "@/components/LanguageSelector.vue";
import LocalStore, { keys } from "@/services/local-store";

const Router = useRouter();
const { t } = useI18n({ useScope: "local" });
const sessionStore = useSessionStore();
const model = reactive({
  username: "",
  password: "",
  rememberMe: !!LocalStore.get(keys.allowStore),
});
const externalErrors = ref({} as Object<string[]>);
const rules = computed(() => {
  return {
    username: { required },
    password: { required },
  };
});
const v$ = useVuelidate(rules, model, { $lazy: true });

async function submitLogin() {
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;
  try {
    await sessionStore.loginAsync(model.username, model.password, model.rememberMe);
    await Router.push(sessionStore.getLastUrl);
  } catch (err: any) {
    const apiErr = err as ErrorApiResponse;
    externalErrors.value = {} as Object<string[]>;
    processError(apiErr, (errs) => { externalErrors.value = errs });
  }
}
</script>

<i18n locale="en">
{
  "welcome": "Welcome to Promasy",
  "signInToContinue": "Sign in to continue",
  "username": "Username",
  "password": "Password",
  "signIn": "Sign In",
  "rememberMe": "Remember me",
  "forgotPassword": "Forgot password?"
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
  "forgotPassword": "Забули пароль?"
}
</i18n>

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

