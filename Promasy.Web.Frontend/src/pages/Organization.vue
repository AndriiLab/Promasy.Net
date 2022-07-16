<template>
  <Loader v-if="loading"></Loader>
  <div v-else class="grid">
    <div class="col-12">
      <div class="card">
        <div class="p-fluid formgrid grid">
          <div class="field col-12">
            <h4>{{ t('organization') }}</h4>
          </div>
          <div class="field col-12">
            <Message v-for="err of externalErrors['']" :severity="'error'" :key="err" :closable="false">{{ err }}</Message>
          </div>
          <ErrorWrap class="field col-12" :errors="v$.name.$errors" :external-errors="externalErrors['Name']">
            <label for="name">{{ t('name') }}</label>
            <Textarea v-model.trim="model.name" :autoResize="true" class="w-full"/>
          </ErrorWrap>
          
          <div class="field col-12">
            <h5>{{ t('contacts') }}</h5>
          </div>
          <ErrorWrap :errors="v$.phoneNumber.$errors" class="field col-12 md:col-4"
                     :external-errors="externalErrors['PhoneNumber']">
            <label for="phoneNumber" >{{ t('phoneNumber') }}</label>
            <InputText id="phoneNumber" type="text" v-model.trim="model.phoneNumber"/>
          </ErrorWrap>

          <ErrorWrap :errors="v$.faxNumber.$errors" class="field col-12 md:col-4"
                     :external-errors="externalErrors['FaxNumber']">
            <label for="faxNumber">{{ t('faxNumber') }}</label>
            <InputText id="faxNumber" type="text" v-model.trim="model.faxNumber"/>
          </ErrorWrap>

          <ErrorWrap :errors="v$.email.$errors" class="field col-12 md:col-4" :external-errors="externalErrors['Email']">
            <label for="email">{{ t('email') }}</label>
            <InputText id="email" type="text" v-model.trim="model.email"/>
          </ErrorWrap>
          
          <div class="field col-12">
            <h5>{{ t('registrationData') }}</h5>
          </div>
          <ErrorWrap :errors="v$.edrpou.$errors" class="field col-12 md:col-4" :external-errors="externalErrors['Edrpou']">
            <label for="edrpou">{{ t('edrpou') }}</label>
            <InputText id="edrpou" type="number" min="0" v-model.trim="model.edrpou"/>
          </ErrorWrap>

          <div class="field col-12">
            <h5>{{ t('address') }}</h5>
          </div>
          <ErrorWrap :errors="v$.postalCode.$errors" class="field col-12 md:col-4" :external-errors="externalErrors['PostalCode']">
            <label for="postalCode">{{ t('postalCode') }}</label>
            <InputText id="postalCode" type="number" min="0" v-model.trim="model.postalCode"/>
          </ErrorWrap>
          <ErrorWrap :errors="v$.country.$errors" class="field col-12 md:col-4" :external-errors="externalErrors['Country']">
            <label for="country">{{ t('country') }}</label>
            <InputText id="country" type="text" v-model.trim="model.country"/>
          </ErrorWrap>
          <ErrorWrap :errors="v$.region.$errors" class="field col-12 md:col-4" :external-errors="externalErrors['Region']">
            <label for="region" >{{ t('region') }}</label>
            <InputText id="region" type="text" v-model.trim="model.region"/>
          </ErrorWrap>

          <ErrorWrap class="field col-12 md:col-4" :errors="v$.cityType.$errors"
                     :external-errors="externalErrors['CityType']">
            <label for="cityType" >{{ t('cityType') }}</label>
            <Dropdown id="cityType" v-model="model.cityType" :options="cityTypes" optionLabel="text"
                      optionValue="value"/>
          </ErrorWrap>
          <ErrorWrap class="field col-12 md:col-4" :errors="v$.city.$errors" :external-errors="externalErrors['City']">
            <label for="city" >{{ t('city') }}</label>
            <InputText id="city" type="text" v-model.trim="model.city"/>
          </ErrorWrap>
          <div class="field col-12 md:col-4"></div>
          <ErrorWrap class="field col-12 md:col-4" :errors="v$.streetType.$errors"
                     :external-errors="externalErrors['StreetType']">
            <label for="streetType" >{{ t('streetType') }}</label>
            <Dropdown id="streetType" v-model="model.streetType" :options="streetTypes" optionLabel="text"
                      optionValue="value" :filter="true"/>
          </ErrorWrap>
          <ErrorWrap class="field col-12 md:col-4" :errors="v$.street.$errors" :external-errors="externalErrors['Street']">
            <label for="street" >{{ t('street') }}</label>
            <InputText id="street" type="text" v-model.trim="model.street"/>
          </ErrorWrap>
          <ErrorWrap :errors="v$.buildingNumber.$errors" class="field col-12 md:col-2"
                     :external-errors="externalErrors['BuildingNumber']">
            <label for="buildingNumber">{{ t('buildingNumber') }}</label>
            <InputText id="buildingNumber" type="text" v-model.trim="model.buildingNumber"/>
          </ErrorWrap>
          <ErrorWrap :errors="v$.internalNumber.$errors" class="field col-12 md:col-2"
                     :external-errors="externalErrors['InternalNumber']">
            <label for="internalNumber">{{ t('internalNumber') }}</label>
            <InputText id="internalNumber" type="text" v-model.trim="model.internalNumber"/>
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
    phoneNumber: { required, numeric, maxLength: maxLength(30) },
    faxNumber: { numeric, maxLength: maxLength(30) },
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
    await Router.push({ name: "NotFound" });
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
  "lastEdit": "Edited",
  "cityType": "Type of city",
  "city": "Name of City",
  "streetType": "Type of Street",
  "street": "Name of Street"
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
  "lastEdit": "Редаговано",
  "cityType": "Тип населенного пункту",
  "city": "Назва населенного пункту",
  "streetType": "Тип вулиці",
  "street": "Назва вулиці"
}
</i18n>