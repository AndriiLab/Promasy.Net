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

        <DataTable :value="[financeSource]" responsiveLayout="scroll" class="mb-3">
          <Column field="number" :header="t('financeSource')" headerStyle="width:25%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('financeSource') }}</span>
              {{ `${slotProps.data.number} - ${slotProps.data.name}` }}
            </template>
          </Column>
          <Column field="unassignedMaterials" :header="t('unassignedName', { name: t('totalMaterials') })"
                  headerStyle="width:25%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('unassignedName', {name: t('totalMaterials')}) }}</span>
              {{ currency(slotProps.data.unassignedMaterials).format() }}
            </template>
          </Column>
          <Column field="unassignedEquipment" :header="t('unassignedName', { name: t('totalEquipment') })"
                  headerStyle="width:25%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('unassignedName', {name: t('totalEquipment')}) }}</span>
              {{ currency(slotProps.data.unassignedEquipment).format() }}
            </template>
          </Column>
          <Column field="unassignedServices" :header="t('unassignedName', { name: t('totalServices') })"
                  headerStyle="width:25%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('unassignedName', {name: t('totalServices')}) }}</span>
              {{ currency(slotProps.data.unassignedServices).format() }}
            </template>
          </Column>
        </DataTable>

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

          <Column field="department" :header="t('department')" :sortable="true"
                  headerStyle="width:10%; min-width:5rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('department') }}</span>
              {{ slotProps.data.department }}
            </template>
          </Column>
          <Column field="subDepartment" :header="t('sub-department', 1)" :sortable="true"
                  headerStyle="width:15%; min-width:5rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('subDepartment', 1) }}</span>
              {{ slotProps.data.subDepartment }}
            </template>
          </Column>
          <Column field="totalMaterials" :header="t('totalMaterials')" :sortable="true"
                  headerStyle="width:10%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('totalMaterials') }}</span>
              {{ currency(slotProps.data.totalMaterials).format() }}
            </template>
          </Column>
          <Column field="leftMaterials" :header="t('leftName', { name: t('totalMaterials') })" :sortable="true"
                  headerStyle="width:10%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('leftName', {name: t('totalMaterials')}) }}</span>
              <span :class="{ 'p-error': parseInt(slotProps.data.leftMaterials) < 0 }">{{
                  currency(slotProps.data.leftMaterials).format()
                }}</span>
            </template>
          </Column>
          <Column field="totalEquipment" :header="t('totalEquipment')" :sortable="true"
                  headerStyle="width:10%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('totalEquipment') }}</span>
              {{ currency(slotProps.data.totalEquipment).format() }}
            </template>
          </Column>
          <Column field="leftEquipment" :header="t('leftName', { name: t('totalEquipment') })" :sortable="true"
                  headerStyle="width:10%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('leftName', {name: t('totalEquipment')}) }}</span>
              <span :class="{ 'p-error': parseInt(slotProps.data.leftEquipment) < 0 }">{{
                  currency(slotProps.data.leftEquipment).format()
                }}</span>
            </template>
          </Column>
          <Column field="totalServices" :header="t('totalServices')" :sortable="true"
                  headerStyle="width:10%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('totalServices') }}</span>
              {{ currency(slotProps.data.totalServices).format() }}
            </template>
          </Column>
          <Column field="leftServices" :header="t('leftName', { name: t('totalServices') })" :sortable="true"
                  headerStyle="width:10%; min-width:10rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('leftName', {name: t('totalServices')}) }}</span>
              <span :class="{ 'p-error': parseInt(slotProps.data.leftServices) < 0 }">{{
                  currency(slotProps.data.leftServices).format()
                }}</span>
            </template>
          </Column>
          <Column headerStyle="min-width:12rem;">
            <template #body="slotProps">
              <router-link icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2"
                           :to="{ name: 'FinanceSubDepartmentsOrders', params: {financeId: slotProps.data.financeSourceId, subDepartmentId: slotProps.data.subDepartmentId, type: 1 }}">
                <Button v-tooltip.left="t('order', 2)" icon="pi pi-shopping-cart"
                        class="p-button-rounded p-button-primary mr-2"/>
              </router-link>
              <Button v-tooltip.left="t('edit')" icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2"
                      @click="edit(slotProps.data)"/>
              <Button v-tooltip.left="t('delete')" icon="pi pi-trash" class="p-button-rounded p-button-warning mt-2"
                      @click="confirmDelete(slotProps.data)"/>
            </template>
          </Column>
        </DataTable>

        <Dialog v-model:visible="itemDialog" :style="{width: '450px'}" :header="t('financeSubDepartmentDetails')"
                :modal="true"
                class="p-fluid">
          <div class="field">
            <Message v-for="err of externalErrors['']" :severity="'error'" :key="err" :closable="false">{{
                err
              }}
            </Message>
            <ErrorWrap :errors="v$.departmentId.$errors" :external-errors="externalErrors['DepartmentId']" class="mt-3">
              <label for="departmentId">{{ t('department') }}</label>
              <DepartmentSelector id="departmentId"
                                  :default-options="departments"
                                  v-model="item.departmentId"
                                  :include-empty="false">
              </DepartmentSelector>
            </ErrorWrap>
            <ErrorWrap :errors="v$.subDepartmentId.$errors" :external-errors="externalErrors['SubDepartmentId']"
                       class="mt-3">
              <label for="subDepartmentId" class="mr-2">{{ t('sub-department') }}</label>
              <SubDepartmentSelector id="subDepartmentId"
                                     :department-id="item.departmentId"
                                     :default-options="subDepartments"
                                     v-model="item.subDepartmentId"
                                     :include-empty="false">
              </SubDepartmentSelector>
            </ErrorWrap>
            <ErrorWrap :errors="v$.totalMaterials.$errors" :external-errors="externalErrors['TotalMaterials']"
                       class="mt-3">
              <label for="totalMaterials">{{ t('totalMaterials') }}</label>
              <FinanceInputWithLimit input-id="totalMaterials" v-model="item.totalMaterials"
                                     :limit="financeSource.unassignedMaterials"
                                     input-placeholder=""></FinanceInputWithLimit>
            </ErrorWrap>
            <ErrorWrap :errors="v$.totalEquipment.$errors" :external-errors="externalErrors['TotalEquipment']"
                       class="mt-3">
              <label for="totalEquipment">{{ t('totalEquipment') }}</label>
              <FinanceInputWithLimit input-id="totalEquipment" v-model="item.totalEquipment"
                                     :limit="financeSource.unassignedEquipment"
                                     input-placeholder=""></FinanceInputWithLimit>
            </ErrorWrap>
            <ErrorWrap :errors="v$.totalServices.$errors" :external-errors="externalErrors['TotalServices']"
                       class="mt-3">
              <label for="totalServices">{{ t('totalServices') }}</label>
              <FinanceInputWithLimit input-id="totalServices" v-model="item.totalServices"
                                     :limit="financeSource.unassignedServices"
                                     input-placeholder=""></FinanceInputWithLimit>
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
import FinanceSubDepartmentsApi, { FinanceSubDepartment } from "@/services/api/finance-sub-departments";
import { useToast } from "primevue/usetoast";
import { useI18n } from "vue-i18n";
import { DataTableSortEvent, DataTablePageEvent } from "primevue/datatable";
import ErrorWrap from "@/components/ErrorWrap.vue";
import FinanceInputWithLimit from "@/components/FinanceInputWithLimit.vue";
import DepartmentSelector from "@/components/DepartmentSelector.vue";
import SubDepartmentSelector from "@/components/SubDepartmentSelector.vue";
import useVuelidate from "@vuelidate/core";
import { required, minValue, maxValue } from "@/i18n/validators";
import currency from "@/utils/currency-utils";
import { useRoute, useRouter } from "vue-router";

const Router = useRouter();
const route = useRoute();
const { t } = useI18n();
const sessionStore = useSessionStore();
const toast = useToast();
const items = ref([] as FinanceSubDepartment[]);
const externalErrors = ref({} as Object<string[]>);
const item = ref(getDefaultItem());
const itemDialog = ref(false);
const deleteItemDialog = ref(false);
const isLoading = ref(false);
const financeSources = ref([] as FinanceSource[]);
const financeSource = ref({} as FinanceSource);
const departments = ref([] as SelectItem<number>[]);
const subDepartments = ref([] as SelectItem<number>[]);
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
    financeSourceId: { required },
    departmentId: { required },
    subDepartmentId: { required },
    totalEquipment: { required, minValue: minValue(0), maxValue: maxValue(financeSource.value.unassignedEquipment) },
    totalMaterials: { required, minValue: minValue(0), maxValue: maxValue(financeSource.value.unassignedMaterials) },
    totalServices: { required, minValue: minValue(0), maxValue: maxValue(financeSource.value.unassignedServices) },
  };
});
const v$ = useVuelidate(rules, item, { $lazy: true });

onMounted(async () => await initAsync());

watch(financeSource, async (fs) => {
  await Router.push({ name: "FinanceSubDepartments", params: { financeId: fs.id } });
});

watch(() => route.params.financeId, async (newId, oldId) => {
  if (oldId !== newId) {
    await initAsync();
  }
});

async function initAsync() {
  await getDataAsync();
}

async function getFinanceSourceListAsync() {
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
  const response = await FinanceSourcesApi.getById(parseInt(route.params.financeId.toString()));
  if (response.success) {
    financeSource.value = response.data!;
    financeSources.value = [ response.data! ];
    await getFinanceSubDepartmentsAsync();
    return;
  } else {
    await Router.push({ name: "NotFound" });
  }
}

async function getFinanceSubDepartmentsAsync() {
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
  departments.value = [];
  subDepartments.value = [];
  item.value = getDefaultItem();
  v$.value.$reset();
  itemDialog.value = true;
}

function edit(selectedItem: FinanceSubDepartment) {
  externalErrors.value = {} as Object<string[]>;
  item.value = { ...selectedItem };
  departments.value = [ { text: item.value.department, value: item.value.departmentId } ];
  subDepartments.value = [ { text: item.value.subDepartment, value: item.value.subDepartmentId } ];
  v$.value.$reset();
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
        totalServices: item.value.totalServices,
      })
      : FinanceSubDepartmentsApi.create({
        financeSourceId: item.value.financeSourceId,
        subDepartmentId: item.value.subDepartmentId,
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
  const response = await FinanceSubDepartmentsApi.delete(item.value.subDepartmentId, item.value.financeSourceId);
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

function getDefaultItem(): FinanceSubDepartment {
  return {
    department: "",
    departmentId: 0,
    editedDate: new Date(),
    editor: "",
    editorId: 0,
    financeSourceId: financeSource.value.id,
    id: 0,
    leftEquipment: 0,
    leftMaterials: 0,
    leftServices: 0,
    subDepartment: "",
    subDepartmentId: 0,
    totalEquipment: 0,
    totalMaterials: 0,
    totalServices: 0,
  };
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
  "leftName": "{name} left",
  "unassignedName": "{name} unassigned"
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
  "leftName": "{name} (залишок)",
  "unassignedName": "{name} (не розподілено)"
}
</i18n>