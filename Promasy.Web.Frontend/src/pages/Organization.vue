<template>
  <Loader v-if="loading"></Loader>
  <div v-else class="grid">
    <div class="col-12">
      <div class="card">
        <h4>{{ t('organization') }}</h4>
        <Message v-for="err of externalErrors['']" :severity="'error'" :key="err" :closable="false">{{ err }}</Message>

        <ErrorWrap class="field grid" :errors="v$.name.$errors" :external-errors="externalErrors['Name']">
          <label for="name" class="col-12 mb-2 md:col-2 md:mb-0">{{ t('name') }}</label>
          <div class="col-12 md:col-10">
            <Textarea v-model.trim="model.name" :autoResize="true" class="w-full"/>
          </div>
        </ErrorWrap>

        <h5>{{ t('contacts') }}</h5>
        <div class="flex justify-content-between flex-wrap">
          <ErrorWrap :errors="v$.phoneNumber.$errors" class="field grid"
                     :external-errors="externalErrors['PhoneNumber']">
            <label for="phoneNumber" class="col-12 mb-2 md:col-4 md:mb-0">{{ t('phoneNumber') }}</label>
            <div class="col-12 md:col-8">
              <InputText id="phoneNumber" type="text" v-model.trim="model.phoneNumber"/>
            </div>
          </ErrorWrap>

          <ErrorWrap :errors="v$.faxNumber.$errors" class="field grid"
                     :external-errors="externalErrors['FaxNumber']">
            <label for="faxNumber" class="col-12 mb-2 md:col-4 md:mb-0">{{ t('faxNumber') }}</label>
            <div class="col-12 md:col-8">
              <InputText id="faxNumber" type="text" v-model.trim="model.faxNumber"/>
            </div>
          </ErrorWrap>
        </div>

        <div class="flex justify-content-between flex-wrap">
          <ErrorWrap :errors="v$.email.$errors" class="field grid" :external-errors="externalErrors['Email']">
            <label for="email" class="col-12 mb-2 md:col-4 md:mb-0">{{ t('email') }}</label>
            <div class="col-12 md:col-8">
              <InputText id="email" type="text" v-model.trim="model.email"/>
            </div>
          </ErrorWrap>
        </div>

        <h5>{{ t('registrationData') }}</h5>
        <div class="flex justify-content-between flex-wrap">
          <ErrorWrap :errors="v$.edrpou.$errors" class="field grid" :external-errors="externalErrors['Edrpou']">
            <label for="edrpou" class="col-12 mb-2 md:col-4 md:mb-0">{{ t('edrpou') }}</label>
            <div class="col-12 md:col-8">
              <InputText id="edrpou" type="number" min="0" v-model.trim="model.edrpou"/>
            </div>
          </ErrorWrap>
        </div>

        <h5>{{ t('address') }}</h5>
        <div class="flex justify-content-between flex-wrap">
          <ErrorWrap :errors="v$.postalCode.$errors" class="field grid" :external-errors="externalErrors['PostalCode']">
            <label for="postalCode" class="col-12 mb-2 md:col-4 md:mb-0">{{ t('postalCode') }}</label>
            <div class="col-12 md:col-8">
              <InputText id="postalCode" type="number" min="0" v-model.trim="model.postalCode"/>
            </div>
          </ErrorWrap>
          <ErrorWrap :errors="v$.country.$errors" class="field grid" :external-errors="externalErrors['Country']">
            <label for="country" class="col-12 mb-2 md:col-4 md:mb-0">{{ t('country') }}</label>
            <div class="col-12 md:col-8">
              <InputText id="country" type="text" v-model.trim="model.country"/>
            </div>
          </ErrorWrap>
        </div>
        <div class="flex justify-content-between flex-wrap">
          <ErrorWrap :errors="v$.region.$errors" class="field grid" :external-errors="externalErrors['Region']">
            <label for="region" class="col-12 mb-2 md:col-4 md:mb-0">{{ t('region') }}</label>
            <div class="col-12 md:col-8">
              <InputText id="region" type="text" v-model.trim="model.region"/>
            </div>
          </ErrorWrap>
        </div>

        <div class="flex justify-content-between flex-wrap">
          <div class="field grid">
            <ErrorWrap class="col-12 mb-2 md:col-4 md:mb-0" :errors="v$.cityType.$errors"
                       :external-errors="externalErrors['CityType']">
              <Dropdown id="cityType" v-model="model.cityType" :options="cityTypes" optionLabel="text"
                        optionValue="value" style="width: 145px;"/>
            </ErrorWrap>
            <ErrorWrap class="col-12 md:col-8" :errors="v$.city.$errors" :external-errors="externalErrors['City']">
              <InputText id="city" type="text" v-model.trim="model.city"/>
            </ErrorWrap>
          </div>

          <div class="field grid" style="margin-right: 10px;">
            <ErrorWrap class="col-12 mb-2 md:col-4 md:mb-0" :errors="v$.streetType.$errors"
                       :external-errors="externalErrors['StreetType']">
              <Dropdown id="streetType" v-model="model.streetType" :options="streetTypes" optionLabel="text"
                        optionValue="value" :filter="true" style="width: 145px;"/>
            </ErrorWrap>
            <ErrorWrap class="col-12 md:col-8" :errors="v$.street.$errors" :external-errors="externalErrors['Street']">
              <InputText id="street" type="text" v-model.trim="model.street"/>
            </ErrorWrap>
          </div>
        </div>

        <div class="flex justify-content-between flex-wrap">
          <ErrorWrap :errors="v$.buildingNumber.$errors" class="field grid"
                     :external-errors="externalErrors['BuildingNumber']">
            <label for="buildingNumber" class="col-12 mb-2 md:col-4 md:mb-0">{{ t('buildingNumber') }}</label>
            <div class="col-12 md:col-8">
              <InputText id="buildingNumber" type="text" v-model.trim="model.buildingNumber"/>
            </div>
          </ErrorWrap>

          <ErrorWrap :errors="v$.internalNumber.$errors" class="field grid"
                     :external-errors="externalErrors['InternalNumber']">
            <label for="internalNumber" class="col-12 mb-2 md:col-4 md:mb-0">{{ t('internalNumber') }}</label>
            <div class="col-12 md:col-8">
              <InputText id="internalNumber" type="text" v-model.trim="model.internalNumber"/>
            </div>
          </ErrorWrap>
        </div>

        <div class="flex justify-content-between flex-wrap mt-5">
          <div v-if="model.editedDate">{{ t('lastEdit') }}:
            <Chip :label="model.editor" icon="pi pi-user"/>
            {{ d(new Date(model.editedDate), 'long') }}
          </div>
          <Button :label="t('save')" icon="pi pi-check" class="p-button" @click="saveAsync"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import ErrorWrap from "@/components/ErrorWrap.vue";
import OrganizationApi, { Organization, UpdateOrganizationRequest } from "@/services/api/organizations";
import { SelectItem } from "@/utils/fetch-utils";
import { useToast } from "primevue/usetoast";
import { ref, onMounted, computed } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";
import useVuelidate from "@vuelidate/core";
import { required, maxLength, email, numeric } from "@/i18n/validators";
import Loader from "@/components/Loader.vue";

const { t, d } = useI18n({ useScope: "local" });
const Router = useRouter();
const route = useRoute();
const toast = useToast();

const cityTypes: SelectItem<number>[] = [];
const streetTypes: SelectItem<number>[] = [];
const externalErrors = ref({} as Object<string[]>);
const model = ref({} as Organization);
const loading = ref(false);
const rules = computed(() => {
  return {
    name: { required, maxLength: maxLength(300) },
    email: { required, email, maxLength: maxLength(100) },
    edrpou: { required, maxLength: maxLength(20) },
    phoneNumber: { required, maxLength: maxLength(30) },
    faxNumber: { maxLength: maxLength(30) },
    country: { required, maxLength: maxLength(100) },
    postalCode: { required, maxLength: maxLength(10) },
    region: { required, maxLength: maxLength(100) },
    city: { required, maxLength: maxLength(100) },
    cityType: { required, numeric },
    street: { required, maxLength: maxLength(100) },
    streetType: { required, numeric },
    buildingNumber: { required, maxLength: maxLength(10) },
    internalNumber: { maxLength: maxLength(10) },
  };
});
const v$ = useVuelidate(rules, model, { $lazy: true });

onMounted(async () => {
  loading.value = true;
  const cityTypesResponse = await OrganizationApi.getCityTypes();
  if (cityTypesResponse.success) {
    cityTypesResponse.data!.forEach(t => cityTypes.push(t));
  }
  const streetTypesResponse = await OrganizationApi.getStreetTypes();
  if (streetTypesResponse.success) {
    streetTypesResponse.data!.forEach(t => streetTypes.push(t));
  }
  const response = await OrganizationApi.getById(parseInt(route.params.organizationId.toString()));
  if (response.success) {
    model.value = response.data!;
    loading.value = false;
  } else {
    await Router.push("/404");
  }
});

async function saveAsync() {
  externalErrors.value = {} as Object<string[]>;
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) {
    return;
  }
  loading.value = true;
  const response = await OrganizationApi.update({ ...model.value } as UpdateOrganizationRequest);
  if (response.success) {
    const dataResponse = await OrganizationApi.getById(parseInt(route.params.organizationId.toString()));
    if (dataResponse.success) {
      model.value = dataResponse.data!;
      loading.value = false;
      toast.add({ severity: "success", summary: t("toast.success"), life: 3000 });
      return;
    }
  } else {
    if (response.error?.errors) {
      externalErrors.value = response.error.errors;
    }
    loading.value = false;
  }
}
</script>

<style lang="css">
.flex .grid label {
  width: 175px;
}

.flex .grid input {
  width: 350px;
}
</style>

<i18n locale="en">
{
  "organization": "Organization",
  "address": "Address",
  "contacts": "Contacts",
  "registrationData": "Registration data",
  "name": "Name",
  "email": "Email",
  "country": "Country",
  "edrpou": "EDRPOU",
  "phoneNumber": "Phone",
  "faxNumber": "Fax",
  "postalCode": "ZIP",
  "region": "Region",
  "buildingNumber": "Building Number",
  "internalNumber": "Internal Building Number",
  "lastEdit": "Edited"
}
</i18n>

<i18n locale="uk">
{
  "organization": "Організація",
  "address": "Адреса",
  "contacts": "Контактні дані",
  "registrationData": "Реєстраційні дані",
  "name": "Назва",
  "email": "Електронна пошта",
  "country": "Країна",
  "edrpou": "ЄДРПОУ",
  "phoneNumber": "Телефон",
  "faxNumber": "Факс",
  "postalCode": "Поштовий індекс",
  "region": "Область",
  "buildingNumber": "Номер будинку",
  "internalNumber": "Корпус",
  "lastEdit": "Редаговано"
}
</i18n>