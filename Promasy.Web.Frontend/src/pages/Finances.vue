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

          <Column field="number" :header="t('number')" :sortable="true" headerStyle="width:10%; min-width:5rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('number') }}</span>
              {{ slotProps.data.number }}
            </template>
          </Column>
          <Column field="name" :header="t('name')" :sortable="true" headerStyle="width:15%; min-width:5rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('name') }}</span>
              {{ slotProps.data.name }}
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
                           :to="{ name: 'FinanceSubDepartments', params: {financeId: slotProps.data.id}}">
                <Button v-tooltip.left="t('sub-department', 2)" icon="pi pi-briefcase" class="p-button-rounded p-button-primary mr-2"/>
              </router-link>
              <Button v-tooltip.left="t('edit')" icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2" @click="edit(slotProps.data)"/>
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
import { useSessionStore } from "@/store/session";
import { SelectItem } from "@/utils/fetch-utils";
import { capitalize } from "@/utils/string-utils";
import { ref, reactive, onMounted, computed, watch } from "vue";
import FinanceSourcesApi, { FinanceSource } from "@/services/api/finances";
import { useToast } from "primevue/usetoast";
import { useI18n } from "vue-i18n";
import { DataTableSortEvent, DataTablePageEvent } from "primevue/datatable";
import ErrorWrap from "@/components/ErrorWrap.vue";
import YearSelector from "@/components/YearSelector.vue";
import useVuelidate from "@vuelidate/core";
import { required, maxLength } from "@/i18n/validators";
import Currency from "currency.js";

const { t } = useI18n();
const uah = (v : string) => Currency(v, { symbol: "₴ ", separator: " ", decimal: "," });
const sessionStore = useSessionStore();
const toast = useToast();
const items = ref([] as FinanceSource[]);
const fundTypes = ref([] as SelectItem<number>[]);
const externalErrors = ref({} as Object<string[]>);
const item = ref({} as FinanceSource);
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
    name: { required, maxLength: maxLength(300) },
  };
});
const v$ = useVuelidate(rules, item, { $lazy: true });


onMounted(async () => {
  await getFundTypes();
  await getDataAsync();
});

watch(() => sessionStore.year, async () => { await getDataAsync(); })

async function getFundTypes(){
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
  item.value = {} as FinanceSource;
  itemDialog.value = true;
}

function edit(selectedItem: FinanceSource) {
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
        end: item.value.end,
        fundType: item.value.fundType,
        id: item.value.id,
        kpkvk: item.value.kpkvk,
        name: item.value.name,
        number: item.value.number,
        start: item.value.start,
        totalEquipment: item.value.totalEquipment,
        totalMaterials: item.value.totalMaterials,
        totalServices: item.value.totalServices
      })
      : FinanceSourcesApi.create({
        end: item.value.end,
        fundType: item.value.fundType,
        kpkvk: item.value.kpkvk,
        name: item.value.name,
        number: item.value.number,
        start: item.value.start,
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
    item.value = {} as FinanceSource;
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
  "financeSources": "Finance Sources",
  "manageFinanceSources": "Finance Sources",
  "name": "Name",
  "number": "Number",
  "financeSourceDetails": "Finance Source Details",
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
  "financeSources": "тем",
  "manageFinanceSources": "темами",
  "number": "Номер",
  "name": "Назва",
  "financeSourceDetails": "Деталі теми",
  "totalMaterials": "Матеріали",
  "totalEquipment": "Обладнання",
  "totalServices": "Послуги",
  "leftEquipment": "Обладнання (залишок)",
  "leftMaterials": "Матеріали (залишок)",
  "leftServices": "Послуги (залишок)"
}
</i18n>