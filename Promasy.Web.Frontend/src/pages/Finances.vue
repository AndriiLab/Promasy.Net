<template>
  <div class="grid">
    <div class="col-12">
      <div class="card">

        <Toolbar class="mb-4">
          <template v-slot:start>
            <div class="my-2">
              <Button :label="t('createDialog.addNew')" icon="pi pi-plus" class="p-button-success mr-2"
                      @click="create"/>
            </div>
          </template>
          <template v-slot:end>
            <label for="currentYear" class="mr-2">{{ t('queryYear') }}</label>
            <YearSelector id="currentYear" style="width: 100px"></YearSelector>
          </template>
        </Toolbar>

        <DataTable ref="dt" :value="items" :lazy="true" :paginator="true" class="p-datatable-sm"
                   :rows="tableData.offset" :totalRecords="tableData.total" :loading="isLoading"
                   @page="onPageAsync($event)" @sort="onSortAsync($event)"
                   paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                   :rowsPerPageOptions="[10,50,100]"
                   :currentPageReportTemplate="t('table.paginationFooter', { itemName: t('financeSources') })"
                   responsiveLayout="scroll">
          <template #header>
            <div class="flex flex-column md:flex-row md:justify-content-between md:align-items-center">
              <h5 class="m-0">{{ t('table.header', {itemName: t('manageFinanceSources')}) }}</h5>
              <span class="block mt-2 md:mt-0 p-input-icon-left">
                  <i class="pi pi-search"/>
                  <InputText v-model.trim="tableData.filter" v-debounce:500="useFilterAsync"
                             :placeholder="t('table.search')"/>
              </span>
            </div>
          </template>

          <Column field="number" :header="t('number')" :sortable="true" headerStyle="width:7%; min-width:5rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('number') }}</span>
              {{ slotProps.data.number }}
            </template>
          </Column>
          <Column field="name" :header="t('name')" :sortable="true" headerStyle="width:10%; min-width:5rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('name') }}</span>
              {{ slotProps.data.name }}
            </template>
          </Column>
          <Column field="totalMaterials" :header="t('totalMaterials')" :sortable="true"
                  headerStyle="width:7%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('totalMaterials') }}</span>
              {{ currency(slotProps.data.totalMaterials).format() }}
            </template>
          </Column>
          <Column field="leftMaterials" :header="t('leftName', { name: t('totalMaterials') })" :sortable="true"
                  headerStyle="width:7%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('leftName', {name: t('totalMaterials')}) }}</span>
              <span :class="{ 'p-error': parseInt(slotProps.data.leftMaterials) < 0 }">{{
                  currency(slotProps.data.leftMaterials).format()
                }}</span>
            </template>
          </Column>
          <Column field="totalEquipment" :header="t('totalEquipment')" :sortable="true"
                  headerStyle="width:7%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('totalEquipment') }}</span>
              {{ currency(slotProps.data.totalEquipment).format() }}
            </template>
          </Column>
          <Column field="leftEquipment" :header="t('leftName', { name: t('totalEquipment') })" :sortable="true"
                  headerStyle="width:7%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('leftName', {name: t('totalEquipment')}) }}</span>
              <span :class="{ 'p-error': parseInt(slotProps.data.leftEquipment) < 0 }">{{
                  currency(slotProps.data.leftEquipment).format()
                }}</span>
            </template>
          </Column>
          <Column field="totalServices" :header="t('totalServices')" :sortable="true"
                  headerStyle="width:7%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('totalServices') }}</span>
              {{ currency(slotProps.data.totalServices).format() }}
            </template>
          </Column>
          <Column field="leftServices" :header="t('leftName', { name: t('totalServices') })" :sortable="true"
                  headerStyle="width:7%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('leftName', {name: t('totalServices')}) }}</span>
              <span :class="{ 'p-error': parseInt(slotProps.data.leftServices) < 0 }">{{
                  currency(slotProps.data.leftServices).format()
                }}</span>
            </template>
          </Column>
          <Column field="start" :header="t('start')" :sortable="true" headerStyle="width:8%; min-width:5rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('start') }}</span>
              {{ d(slotProps.data.start, 'short') }}
            </template>
          </Column>
          <Column field="end" :header="t('end')" :sortable="true" headerStyle="width:8%; min-width:5rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('end') }}</span>
              {{ d(slotProps.data.end, 'short') }}
            </template>
          </Column>
          <Column headerStyle="min-width:12rem;">
            <template #body="slotProps">
              <router-link icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2"
                           :to="{ name: 'FinanceSubDepartments', params: {financeId: slotProps.data.id}}">
                <Button v-tooltip.left="t('sub-department', 2)" icon="pi pi-briefcase"
                        class="p-button-rounded p-button-primary mr-2"/>
              </router-link>
              <Button v-tooltip.left="t('edit')" icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2"
                      @click="edit(slotProps.data)"/>
              <Button v-tooltip.left="t('delete')" icon="pi pi-trash" class="p-button-rounded p-button-warning mt-2"
                      @click="confirmDelete(slotProps.data)"/>
            </template>
          </Column>
        </DataTable>


        <Dialog v-model:visible="itemDialog" :style="{width: '450px'}" :header="t('financeSourceDetails')" :modal="true"
                class="p-fluid">
          <div class="field">
            <Message v-for="err of externalErrors['']" :severity="'error'" :key="err" :closable="false">{{
                err
              }}
            </Message>
            <ErrorWrap :errors="v$.number.$errors" :external-errors="externalErrors['Number']">
              <label for="number">{{ t('number') }}</label>
              <InputText id="number" v-model.trim="item.number" required="true" autofocus/>
            </ErrorWrap>
            <ErrorWrap :errors="v$.name.$errors" :external-errors="externalErrors['Name']" class="mt-3">
              <label for="name">{{ t('name') }}</label>
              <InputText id="name" v-model.trim="item.name" required="true"/>
            </ErrorWrap>
            <ErrorWrap :errors="v$.fundType.$errors" :external-errors="externalErrors['FundType']" class="mt-3">
              <label for="fundType">{{ t('fundType') }}</label>
              <Dropdown id="fundType" :options="fundTypes" v-model="item.fundType" option-label="text"
                        option-value="value"></Dropdown>
            </ErrorWrap>
            <ErrorWrap :errors="v$.totalMaterials.$errors" :external-errors="externalErrors['TotalMaterials']"
                       class="mt-3">
              <label for="totalMaterials">{{ t('totalMaterials') }}</label>
              <FinanceInput input-id="totalMaterials" v-model="item.totalMaterials" input-placeholder=""></FinanceInput>
            </ErrorWrap>
            <ErrorWrap :errors="v$.totalEquipment.$errors" :external-errors="externalErrors['TotalEquipment']"
                       class="mt-3">
              <label for="totalEquipment">{{ t('totalEquipment') }}</label>
              <FinanceInput input-id="totalEquipment" v-model="item.totalEquipment" input-placeholder=""></FinanceInput>
            </ErrorWrap>
            <ErrorWrap :errors="v$.totalServices.$errors" :external-errors="externalErrors['TotalServices']"
                       class="mt-3">
              <label for="totalServices">{{ t('totalServices') }}</label>
              <FinanceInput input-id="totalServices" v-model="item.totalServices" input-placeholder=""></FinanceInput>
            </ErrorWrap>
            <ErrorWrap :errors="v$.start.$errors" :external-errors="externalErrors['Start']" class="mt-3">
              <label for="start">{{ t('start') }}</label>
              <Calendar id="start" v-model="item.start" dateFormat="yy-mm-dd"></Calendar>
            </ErrorWrap>
            <ErrorWrap :errors="v$.end.$errors" :external-errors="externalErrors['End']" class="mt-3">
              <label for="end">{{ t('end') }}</label>
              <Calendar id="end" v-model="item.end" dateFormat="yy-mm-dd"></Calendar>
            </ErrorWrap>
            <ErrorWrap :errors="v$.kpkvk.$errors" :external-errors="externalErrors['Kpkvk']" class="mt-3">
              <label for="kpkvk">{{ t('kpkvk') }}</label>
              <InputText id="kpkvk" v-model.trim="item.kpkvk" required="true"/>
            </ErrorWrap>
          </div>

          <template #footer>
            <Button :label="t('cancel')" icon="pi pi-times" class="p-button-text" @click="closeItemDialog"/>
            <Button :label="t('save')" icon="pi pi-check" class="p-button-text" @click="saveAsync"/>
          </template>
        </Dialog>

        <Dialog v-model:visible="deleteItemDialog" :style="{width: '450px'}" :header="t('deleteDialog.header')"
                :modal="true">
          <Message v-for="err of externalErrors['']" :severity="'error'" :key="err" :closable="false">{{
              err
            }}
          </Message>
          <div class="flex align-items-center justify-content-center">
            <i class="pi pi-exclamation-triangle mr-3" style="font-size: 2rem"/>
            <span v-if="item">{{ t('deleteDialog.text') }} <b>{{ item.name }}</b>?</span>
          </div>
          <template #footer>
            <Button :label="t('no')" icon="pi pi-times" class="p-button-text" @click="closeDeleteItemDialog"/>
            <Button :label="t('yes')" icon="pi pi-check" class="p-button-text" @click="deleteItemAsync"/>
          </template>
        </Dialog>

      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { FinanceSubDepartment } from "@/services/api/finance-sub-departments";
import { useSessionStore } from "@/store/session";
import { formatAsDate } from "@/utils/date-utils";
import { SelectItem } from "@/utils/fetch-utils";
import { capitalize } from "@/utils/string-utils";
import { ref, reactive, onMounted, computed, watch } from "vue";
import FinanceSourcesApi, { FinanceSource } from "@/services/api/finances";
import { useToast } from "primevue/usetoast";
import { useI18n } from "vue-i18n";
import { DataTableSortEvent, DataTablePageEvent } from "primevue/datatable";
import ErrorWrap from "@/components/ErrorWrap.vue";
import FinanceInput from "@/components/FinanceInput.vue";
import YearSelector from "@/components/YearSelector.vue";
import useVuelidate from "@vuelidate/core";
import { required, maxLength, minValue } from "@/i18n/validators";
import currency from "@/utils/currency-utils";

const { t, d } = useI18n();
const sessionStore = useSessionStore();
const toast = useToast();
const items = ref([] as FinanceSource[]);
const fundTypes = ref([] as SelectItem<number>[]);
const externalErrors = ref({} as Object<string[]>);
const item = ref(getDefaultItem());
const itemDialog = ref(false);
const deleteItemDialog = ref(false);
const isLoading = ref(false);
const tableData: TablePagingData = reactive({
  page: 1,
  offset: 10,
  filter: undefined,
  orderBy: undefined,
  descending: undefined,
  total: 0,
});

const rules = computed(() => {
  return {
    number: { required, maxLength: maxLength(100) },
    name: { required, maxLength: maxLength(300) },
    kpkvk: { required, maxLength: maxLength(10) },
    fundType: { required },
    totalEquipment: { required, minValue: minValue(0) },
    totalMaterials: { required, minValue: minValue(0) },
    totalServices: { required, minValue: minValue(0) },
    start: { required },
    end: { required },
  };
});
const v$ = useVuelidate(rules, item, { $lazy: true });


onMounted(async () => {
  await getFundTypes();
  await getDataAsync();
});

watch(() => sessionStore.year, async () => {
  await getDataAsync();
});

async function getFundTypes() {
  const response = await FinanceSourcesApi.getFundTypes();
  if (response.success) {
    fundTypes.value = response.data!;
  }
}

async function useFilterAsync() {
  await getDataAsync();
}

async function getDataAsync() {
  isLoading.value = true;
  const response = await FinanceSourcesApi.getList(sessionStore.year, true, tableData.page, tableData.offset, tableData.filter, tableData.orderBy, tableData.descending);
  if (response.success) {
    items.value = response.data?.collection ?? [] as FinanceSource[];
    tableData.page = response.data?.page ?? tableData.page;
    tableData.total = response.data?.total ?? 0;
    isLoading.value = false;
    return;
  }
  toast.add({ severity: "error", summary: t("toast.error"), detail: t("table.loadError"), life: 3000 });
  isLoading.value = false;
}

async function onPageAsync(event: DataTablePageEvent) {
  tableData.page = event.page + 1;
  tableData.offset = event.rows;
  await getDataAsync();
}

async function onSortAsync(event: DataTableSortEvent) {
  tableData.orderBy = capitalize(event.sortField as string);
  tableData.descending = event.sortOrder === 1 ? undefined : true;
  await getDataAsync();
}

function create() {
  externalErrors.value = {} as Object<string[]>;
  item.value = getDefaultItem();
  v$.value.$reset();
  itemDialog.value = true;
}

function edit(selectedItem: FinanceSource) {
  externalErrors.value = {} as Object<string[]>;
  item.value = { ...selectedItem };
  item.value.start = new Date(item.value.start);
  item.value.end = new Date(item.value.end);
  v$.value.$reset();
  itemDialog.value = true;
}

function closeItemDialog() {
  itemDialog.value = false;
}

function closeDeleteItemDialog() {
  deleteItemDialog.value = false;
}

function confirmDelete(selectedItem: FinanceSource) {
  externalErrors.value = {} as Object<string[]>;
  item.value = selectedItem;
  deleteItemDialog.value = true;
}

async function saveAsync() {
  externalErrors.value = {} as Object<string[]>;
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) {
    return;
  }
  const response = await (item.value.id
      ? FinanceSourcesApi.update({
        end: formatAsDate(item.value.end),
        fundType: item.value.fundType,
        id: item.value.id,
        kpkvk: item.value.kpkvk,
        name: item.value.name,
        number: item.value.number,
        start: formatAsDate(item.value.start),
        totalEquipment: item.value.totalEquipment,
        totalMaterials: item.value.totalMaterials,
        totalServices: item.value.totalServices,
      })
      : FinanceSourcesApi.create({
        end: formatAsDate(item.value.end),
        fundType: item.value.fundType,
        kpkvk: item.value.kpkvk,
        name: item.value.name,
        number: item.value.number,
        start: formatAsDate(item.value.start),
        totalEquipment: item.value.totalEquipment,
        totalMaterials: item.value.totalMaterials,
        totalServices: item.value.totalServices,
      }));
  if (response.success) {
    itemDialog.value = false;
    await getDataAsync();
    toast.add({ severity: "success", summary: t("toast.success"), life: 3000 });
    return;
  }
  if (response.error?.errors) {
    externalErrors.value = response.error.errors;
  }
}

async function deleteItemAsync() {
  externalErrors.value = {} as Object<string[]>;
  const response = await FinanceSourcesApi.delete(item.value.id);
  if (response.success) {
    item.value = getDefaultItem();
    deleteItemDialog.value = false;
    await getDataAsync();
    toast.add({ severity: "success", summary: t("toast.success"), life: 3000 });
    return;
  }
  if (response.error?.errors) {
    externalErrors.value = response.error.errors;
  }
}

function getDefaultItem(): FinanceSource {
  return {
    editedDate: new Date(),
    editor: "",
    editorId: 0,
    id: 0,
    kpkvk: "",
    leftEquipment: 0,
    leftMaterials: 0,
    leftServices: 0,
    name: "",
    number: "",
    unassignedEquipment: 0,
    unassignedMaterials: 0,
    unassignedServices: 0,
    fundType: 1,
    totalEquipment: 0,
    totalMaterials: 0,
    totalServices: 0,
    start: new Date(),
    end: new Date(new Date().getFullYear(), 11, 31)
  };
}
</script>

<i18n locale="en">
{
  "financeSources": "Finance Sources",
  "manageFinanceSources": "Finance Sources",
  "name": "Name",
  "number": "Number",
  "financeSourceDetails": "Finance Source Details",
  "totalMaterials": "Materials",
  "totalEquipment": "Equipment",
  "totalServices": "Services",
  "leftName": "{name} left",
  "start": "Start date",
  "end": "End date",
  "fundType": "Finance type",
  "kpkvk": "KPKVK"
}
</i18n>

<i18n locale="uk">
{
  "financeSources": "тем",
  "manageFinanceSources": "темами",
  "number": "Номер",
  "name": "Назва",
  "financeSourceDetails": "Деталі теми",
  "totalMaterials": "Матеріали",
  "totalEquipment": "Обладнання",
  "totalServices": "Послуги",
  "leftName": "{name} (залишок)",
  "start": "Дата початку",
  "end": "Дата завершення",
  "fundType": "Тип фінансування",
  "kpkvk": "КПКВК"
}
</i18n>