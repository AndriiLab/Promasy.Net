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
            <label for="financeSource" class="mr-2">{{ t('financeSource') }}</label>
            <Dropdown id="financeSource" v-model="financeSource" :options="financeSources"
                      v-on:before-show="getFinanceSourceListAsync" class="w-17rem"
                      :optionLabel="(d) => { return `${d.number} - ${d.name}`; }" :loading="isLoading"/>
          </template>
        </Toolbar>

        <DataTable ref="dt" :value="items" :lazy="true" :paginator="true" class="p-datatable-sm"
                   :rows="tableData.offset" :totalRecords="tableData.total" :loading="isLoading"
                   @page="onPageAsync($event)" @sort="onSortAsync($event)"
                   paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                   :rowsPerPageOptions="[10,50,100]"
                   :currentPageReportTemplate="t('table.paginationFooter', { itemName: t('financeSubDepartments') })"
                   responsiveLayout="scroll">
          <template #header>
            <div class="flex flex-column md:flex-row md:justify-content-between md:align-items-center">
              <h5 class="m-0">{{ t('table.header', {itemName: t('manageFinanceSubDepartments')}) }}</h5>
              <span class="block mt-2 md:mt-0 p-input-icon-left">
                  <i class="pi pi-search"/>
                  <InputText v-model.trim="tableData.filter" v-debounce:500="useFilterAsync"
                             :placeholder="t('table.search')"/>
              </span>
            </div>
          </template>

          <Column field="department" :header="t('department')" :sortable="true" headerStyle="width:10%; min-width:5rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('department') }}</span>
              {{ slotProps.data.department }}
            </template>
          </Column>
          <Column field="subDepartment" :header="t('sub-department', 1)" :sortable="true" headerStyle="width:15%; min-width:5rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('subDepartment', 1) }}</span>
              {{ slotProps.data.subDepartment }}
            </template>
          </Column>
          <Column field="totalMaterials" :header="t('totalMaterials')" :sortable="true" headerStyle="width:10%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('totalMaterials') }}</span>
              {{ uah(slotProps.data.totalMaterials).format() }}
            </template>
          </Column>
          <Column field="leftMaterials" :header="t('leftMaterials')" :sortable="true" headerStyle="width:10%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('leftMaterials') }}</span>
              <span :class="{ 'p-error': parseInt(slotProps.data.leftMaterials) < 0 }">{{ uah(slotProps.data.leftMaterials).format() }}</span>
            </template>
          </Column>
          <Column field="totalEquipment" :header="t('totalEquipment')" :sortable="true" headerStyle="width:10%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('totalEquipment') }}</span>
              {{ uah(slotProps.data.totalEquipment).format() }}
            </template>
          </Column>
          <Column field="leftEquipment" :header="t('leftEquipment')" :sortable="true" headerStyle="width:10%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('leftEquipment') }}</span>
              <span :class="{ 'p-error': parseInt(slotProps.data.leftEquipment) < 0 }">{{ uah(slotProps.data.leftEquipment).format() }}</span>
            </template>
          </Column>
          <Column field="totalServices" :header="t('totalServices')" :sortable="true" headerStyle="width:10%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('totalServices') }}</span>
              {{ uah(slotProps.data.totalServices).format() }}
            </template>
          </Column>
          <Column field="leftServices" :header="t('leftServices')" :sortable="true" headerStyle="width:10%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('leftServices') }}</span>
              <span :class="{ 'p-error': parseInt(slotProps.data.leftServices) < 0 }">{{ uah(slotProps.data.leftServices).format() }}</span>
            </template>
          </Column>
          <Column headerStyle="min-width:12rem;">
            <template #body="slotProps">
              <router-link icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2"
                           :to="{ name: 'FinanceSubDepartmentsOrders', params: {financeId: slotProps.data.financeSourceId, subDepartmentId: slotProps.data.subDepartmentId, type: 1 }}">
                <Button v-tooltip.left="t('order', 2)" icon="pi pi-shopping-cart" class="p-button-rounded p-button-primary mr-2"/>
              </router-link>
              <Button v-tooltip.left="t('edit')" icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2" @click="edit(slotProps.data)"/>
              <Button v-tooltip.left="t('delete')" icon="pi pi-trash" class="p-button-rounded p-button-warning mt-2"
                      @click="confirmDelete(slotProps.data)"/>
            </template>
          </Column>
        </DataTable>

        <Dialog v-model:visible="itemDialog" :style="{width: '450px'}" :header="t('financeSubDepartmentDetails')" :modal="true"
                class="p-fluid">
          <div class="field">
            <Message v-for="err of externalErrors['']" :severity="'error'" :key="err" :closable="false">{{
                err
              }}
            </Message>
            <ErrorWrap :errors="v$.name.$errors" :external-errors="externalErrors['Name']">
              <label for="name">{{ t('name') }}</label>
              <InputText id="name" v-model.trim="item.name" required="true" autofocus/>
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
import DepartmentsApi from "@/services/api/departments";
import SubDepartmentsApi from "@/services/api/sub-departments";
import { useSessionStore } from "@/store/session";
import { SelectItem } from "@/utils/fetch-utils";
import { capitalize } from "@/utils/string-utils";
import { ref, reactive, onMounted, computed, watch } from "vue";
import FinanceSourcesApi, { FinanceSource } from "@/services/api/finances";
import FinanceSubDepartmentsApi, { FinanceSubDepartment } from "@/services/api/finance-sub-departments";
import { useToast } from "primevue/usetoast";
import { useI18n } from "vue-i18n";
import { DataTableSortEvent, DataTablePageEvent } from "primevue/datatable";
import ErrorWrap from "@/components/ErrorWrap.vue";
import useVuelidate from "@vuelidate/core";
import { required, maxLength } from "@/i18n/validators";
import Currency from "currency.js";
import { useRoute, useRouter } from "vue-router";

const Router = useRouter();
const route = useRoute();
const { t, } = useI18n();
const uah = (v : string) => Currency(v, { symbol: "₴ ", separator: " ", decimal: "," });
const sessionStore = useSessionStore();
const toast = useToast();
const items = ref([] as FinanceSubDepartment[]);
const externalErrors = ref({} as Object<string[]>);
const item = ref({} as FinanceSubDepartment);
const itemDialog = ref(false);
const deleteItemDialog = ref(false);
const isLoading = ref(false);
const financeSourceId = ref(0);
const financeSources = ref([] as FinanceSource[]);
const financeSource = ref({ } as FinanceSource);
const tableData: TablePagingData = reactive({
  page: 1,
  offset: 10,
  filter: undefined,
  orderBy: undefined,
  descending: undefined,
  total: 0,
});

const rules = computed(() => {
  return { };
});
const v$ = useVuelidate(rules, item, { $lazy: true });


onMounted(async () => await initAsync());

watch(financeSource, async (fs) => {
  await Router.push({ name: "FinanceSubDepartments", params: { financeId: fs.id } });
});

watch(() => route.params.financeId, async (newId, oldId) => {
  if(oldId !== newId){
    await initAsync();
  }
});

async function initAsync() {
  isLoading.value = true;
  const response = await FinanceSourcesApi.getById(parseInt(route.params.financeId.toString()));
  if(response.success){
    financeSource.value = response.data!;
    financeSources.value = [ response.data! ];
    await getDataAsync();
    return;
  } else {
    await Router.push({ name: "NotFound" });
  }
}

async function getFinanceSourceListAsync(){
  isLoading.value = true;
  const response = await FinanceSourcesApi.getList(sessionStore.year, false, 1, 1000, undefined, "Id", undefined);
  if (response) {
    financeSources.value = response.data!.collection;
  }
  isLoading.value = false;
}

async function useFilterAsync() {
  await getDataAsync();
}

async function getDataAsync() {
  isLoading.value = true;
  const response = await FinanceSubDepartmentsApi.getList(financeSource.value.id, true, tableData.page, tableData.offset, tableData.filter, tableData.orderBy, tableData.descending);
  if (response.success) {
    items.value = response.data?.collection ?? [] as FinanceSubDepartment[];
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
  item.value = {} as FinanceSubDepartment;
  itemDialog.value = true;
}

function edit(selectedItem: FinanceSubDepartment) {
  externalErrors.value = {} as Object<string[]>;
  item.value = { ...selectedItem };
  itemDialog.value = true;
}

function closeItemDialog() {
  itemDialog.value = false;
}

function closeDeleteItemDialog() {
  deleteItemDialog.value = false;
}

function confirmDelete(selectedItem: FinanceSubDepartment) {
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
      ? FinanceSubDepartmentsApi.update({
        id: item.value.id,
        financeSourceId: item.value.financeSourceId,
        subDepartmentId: item.value.subDepartmentId,
        totalEquipment: item.value.totalEquipment,
        totalMaterials: item.value.totalMaterials,
        totalServices: item.value.totalServices
      })
      : FinanceSubDepartmentsApi.create({
        financeSourceId: item.value.financeSourceId,
        subDepartmentId: item.value.subDepartmentId,
        totalEquipment: item.value.totalEquipment,
        totalMaterials: item.value.totalMaterials,
        totalServices: item.value.totalServices
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
    item.value = {} as FinanceSubDepartment;
    deleteItemDialog.value = false;
    await getDataAsync();
    toast.add({ severity: "success", summary: t("toast.success"), life: 3000 });
    return;
  }
  if (response.error?.errors) {
    externalErrors.value = response.error.errors;
  }
}
</script>

<i18n locale="en">
{
  "financeSource": "Finance Source",
  "financeSubDepartments": "Finances by sub-departments",
  "manageFinanceSubDepartments": "Finances by sub-departments",
  "name": "Name",
  "number": "Number",
  "financeSubDepartmentDetails": "Finances by sub-departments Details",
  "totalMaterials": "Materials",
  "totalEquipment": "Equipment",
  "totalServices": "Services",
  "leftEquipment": "Equipment left",
  "leftMaterials": "Materials left",
  "leftServices": "Services left"
}
</i18n>

<i18n locale="uk">
{
  "financeSource": "Тема",
  "financeSubDepartments": "тем по підрозділам",
  "manageFinanceSubDepartments": "темами по підрозділам",
  "number": "Номер",
  "name": "Назва",
  "financeSubDepartmentDetails": "Деталі теми по підрозділу",
  "totalMaterials": "Матеріали",
  "totalEquipment": "Обладнання",
  "totalServices": "Послуги",
  "leftEquipment": "Обладнання (залишок)",
  "leftMaterials": "Матеріали (залишок)",
  "leftServices": "Послуги (залишок)"
}
</i18n>