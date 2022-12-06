<template>
  <div class="grid">
    <div class="col-12">
      <div class="card">

        <Toolbar class="mb-4">
          <template v-slot:start>
            <div class="my-2">
              <router-link :to="{ name: 'OrderNew' }">
                <Button :label="t('createDialog.addNew')" icon="pi pi-plus" class="p-button-success mr-2"/>
              </router-link>
            </div>
          </template>
          <template v-slot:end>
            <label for="currentYear" class="mr-2">{{ t('queryYear') }}</label>
            <YearSelector id="currentYear" style="width: 100px" :disabled="financeSourceId > 0"></YearSelector>
            <label for="financeSource" class="ml-3 mr-2">{{ t('financeSource') }}</label>
            <Dropdown id="financeSource" v-model="financeSourceId" :options="financeSources"
                      v-on:before-show="getFinanceSourceListAsync" class="w-17rem"
                      optionLabel="text" optionValue="value" :loading="isLoading"/>
            <label for="type" class="ml-3 mr-2">{{ t('type') }}</label>
            <Dropdown id="type" v-model="typeId" :options="types" class="w-11rem"
                      optionLabel="text" optionValue="value"/>
          </template>
        </Toolbar>

        <DataTable ref="dt" :value="items" :lazy="true" :paginator="true" class="p-datatable-sm"
                   :rows="tableData.offset" :totalRecords="tableData.total" :loading="isLoading"
                   @page="onPageAsync($event)" @sort="onSortAsync($event)"
                   paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                   :rowsPerPageOptions="[10,50,100]"
                   :currentPageReportTemplate="t('table.paginationFooter', { itemName: t('orders') })"
                   responsiveLayout="scroll">
          <template #header>
            <div class="flex flex-column md:flex-row md:justify-content-between md:align-items-center">

              <h5 class="m-0">{{ t('table.header', {itemName: t('manageOrders')}) }}</h5>
              <div>
                <label for="department" class="mr-2 ml-7">{{ t('department') }}</label>
                <DepartmentSelector id="department"
                                    :default-options="departments"
                                    v-model="departmentId"
                                    :include-empty="true"
                                    class="w-23rem">
                </DepartmentSelector>
              </div>
              <div>
                <label for="subDepartment" class="mr-2">{{ t('sub-department') }}</label>
                <SubDepartmentSelector id="subDepartment"
                                       :department-id="departmentId"
                                       :default-options="subDepartments"
                                       v-model="subDepartmentId"
                                       :include-empty="true"
                                       class="w-20rem">
                </SubDepartmentSelector>
              </div>

              <div class="block mt-2 md:mt-0 p-input-icon-left">
                <i class="pi pi-search"/>
                <InputText v-model.trim="tableData.filter" v-debounce:500="useFilterAsync"
                           :placeholder="t('table.search')"/>
              </div>
            </div>
          </template>

          <Column field="description" :header="t('description')" :sortable="true"
                  headerStyle="width:35%; min-width:5rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('description') }}</span>
              {{ slotProps.data.description }}
            </template>
          </Column>
          <Column field="total" :header="t('total')" :sortable="true"
                  headerStyle="width:15%; min-width:5rem;" class="text-right">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('total') }}</span>
              {{ currency(slotProps.data.total).format() }}
            </template>
          </Column>
          <Column field="status" :header="t('status')"
                  headerStyle="width:10%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('status') }}</span>
              <OrderStatusBadge :status="slotProps.data.status" :statuses="availableStatuses"></OrderStatusBadge>
            </template>
          </Column>
          <Column field="editor" :header="t('table.columns.editor')" headerStyle="width:15%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('table.columns.editor') }}</span>
              <Chip :label="slotProps.data.editor" icon="pi pi-user"/>
            </template>
          </Column>
          <Column field="editedDate" :header="t('table.columns.editDate')" headerStyle="width:25%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('table.columns.editDate') }}</span>
              {{ d(new Date(slotProps.data.editedDate), 'long') }}
            </template>
          </Column>
          <Column headerStyle="min-width:10rem;">
            <template #body="slotProps">
              <router-link icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2"
                           :to="{ name: 'OrderView', params: { orderId: slotProps.data.id }}">
                <Button v-tooltip.left="t('edit')" icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2"/>
              </router-link>
              <Button v-tooltip.left="t('delete')" icon="pi pi-trash" class="p-button-rounded p-button-warning mt-2"
                      @click="confirmDelete(slotProps.data)"/>
            </template>
          </Column>
          <template #footer>
            <div class="flex justify-content-end">
              <span v-if="tableData.leftAmount" class="mr-7">{{t('leftAmount')}}: {{currency(tableData.leftAmount).format()}}</span>
              <span>{{t('total')}}: {{currency(tableData.spentAmount).format()}}</span>
            </div>
          </template>
        </DataTable>

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
import FinanceSourcesApi from "@/services/api/finances";
import OrdersApi, { Order, OrderShort } from "@/services/api/orders";
import processError from "@/utils/error-response-utils";
import { RouteLocationRaw, useRoute, useRouter } from "vue-router";
import { useSessionStore } from "@/store/session";
import { SelectItem } from "@/utils/fetch-utils";
import { capitalize } from "@/utils/string-utils";
import { ref, reactive, onBeforeMount, computed, watch } from "vue";
import { useToast } from "primevue/usetoast";
import { debounce } from "vue-debounce";
import { useI18n } from "vue-i18n";
import { DataTableSortEvent, DataTablePageEvent } from "primevue/datatable";
import currency from "@/utils/currency-utils";
import YearSelector from "@/components/YearSelector.vue";
import OrderStatusBadge from "@/components/OrderStatusBadge.vue";
import DepartmentSelector from "@/components/DepartmentSelector.vue";
import SubDepartmentSelector from "@/components/SubDepartmentSelector.vue";

const Router = useRouter();
const route = useRoute();
const { t, d } = useI18n();
const sessionStore = useSessionStore();
const toast = useToast();
const items = ref([] as OrderShort[]);
const externalErrors = ref({} as Object<string[]>);
const item = ref({} as Order);
const deleteItemDialog = ref(false);
const isLoading = ref(false);
const typeId = ref(0);
const types = ref([] as SelectItem<number>[]);
const availableStatuses = ref([] as SelectItem<number>[]);
const financeSourceId = ref(0);
const financeSources = ref([ getDefaultSelectItem() ] as SelectItem<number>[]);
const departmentId = ref(0);
const departments = ref([] as SelectItem<number>[]);
const subDepartmentId = ref(0);
const subDepartments = ref([] as SelectItem<number>[]);
const tableData: OrderTablePagingData = reactive({
  page: 1,
  offset: 10,
  filter: undefined,
  orderBy: undefined,
  descending: undefined,
  total: 0,
  spentAmount: 0,
  leftAmount: undefined,
});

onBeforeMount(async () => {
  isLoading.value = true;
  await setAvailableStatusesAsync();
  await setTypesAsync();
  await debouncedInitAsync();
});

watch(() => route.params, async (n, o) => {
  if (n.departmentId !== o.departmentId ||
      n.subDepartmentId !== o.subDepartmentId ||
      n.financeId !== o.financeId ||
      n.type !== o.type) {
    await debouncedInitAsync();
  }
});

watch(() => sessionStore.year, async () => await debouncedInitAsync());
watch(typeId, async () => await debouncedSetPathAsync());
watch(departmentId, async () => await debouncedSetPathAsync());
watch(subDepartmentId, async () => await debouncedSetPathAsync());
watch(financeSourceId, async () => await debouncedSetPathAsync());

async function initAsync() {
  if (!route.fullPath.includes("/orders/type")) {
    return;
  }
  let isSuccess = true;
  isLoading.value = true;
  typeId.value = parseInt(route.params.type?.toString());
  if (isNaN(typeId.value) || types.value.every(i => i.value !== typeId.value)) {
    isSuccess = false;
  }
  const selectedDepartmentId = parseInt(route.params.departmentId?.toString());
  if (!isNaN(selectedDepartmentId) && isSuccess) {
    isSuccess = await setDepartmentAsync(selectedDepartmentId);
  }
  const selectedSubDepartmentId = parseInt(route.params.subDepartmentId?.toString());
  if (!isNaN(selectedDepartmentId) && !isNaN(selectedSubDepartmentId) && isSuccess) {
    isSuccess = await setSubDepartmentAsync(selectedSubDepartmentId);
  }
  const selectedFinanceSourceId = parseInt(route.params.financeId?.toString());
  if (!isNaN(selectedFinanceSourceId) && isSuccess) {
    isSuccess = await setFinanceSourceAsync(selectedFinanceSourceId);
  }
  if (isSuccess) {
    await getDataAsync();
  } else {
    await Router.push({ name: "NotFound" });
  }
}

const debouncedInitAsync =
    debounce(async () => await initAsync(), 400);

const debouncedSetPathAsync =
    debounce(async () => await setPathAsync(), 400);

async function setTypesAsync() {
  const response = await OrdersApi.getExistingOrderTypes();
  if (response.success) {
    types.value = response.data!;
  }
}

async function setAvailableStatusesAsync() {
  const response = await OrdersApi.getExistingOrderStatuses();
  if (response.success) {
    availableStatuses.value = response.data!;
  }
}

async function setDepartmentAsync(id: number) {
  const response = await DepartmentsApi.getById(id, sessionStore.user!.organizationId);
  if (response.success) {
    departments.value = [ { text: response.data!.name, value: response.data!.id } ];
    departmentId.value = response.data!.id;
  }
  return response.success;
}

async function setSubDepartmentAsync(id: number) {
  const response = await SubDepartmentsApi.getById(id, sessionStore.user!.organizationId, departmentId.value);
  if (response.success) {
    subDepartments.value = [ { text: response.data!.name, value: response.data!.id } ];
    subDepartmentId.value = response.data!.id;
  }
  return response.success;
}

async function setFinanceSourceAsync(id: number) {
  const response = await FinanceSourcesApi.getById(id);
  if (response) {
    financeSources.value = [ { text: response.data!.number, value: response.data!.id } ];
    financeSourceId.value = response.data!.id;
    const fsYear = parseInt(response.data!.start.toString().substring(0, 4));
    if (fsYear !== sessionStore.year) {
      sessionStore.setYear(fsYear);
    }
  }

  return response.success;
}

async function setPathAsync() {
  let location: RouteLocationRaw;
  if (financeSourceId.value) {
    if (subDepartmentId.value) {
      location = {
        name: "FinanceSubDepartmentsOrders",
        params: { type: typeId.value, subDepartmentId: subDepartmentId.value, financeId: financeSourceId.value },
      };
    } else if (departmentId.value) {
      location = {
        name: "FinanceDepartmentOrders",
        params: { type: typeId.value, departmentId: departmentId.value, financeId: financeSourceId.value },
      };
    } else {
      location = { name: "FinanceOrders", params: { type: typeId.value, financeId: financeSourceId.value } };
    }
  } else if (departmentId.value) {
    if (subDepartmentId.value) {
      location = {
        name: "SubDepartmentsOrders",
        params: { type: typeId.value, subDepartmentId: subDepartmentId.value, departmentId: departmentId.value },
      };
    } else {
      location = { name: "DepartmentOrders", params: { type: typeId.value, departmentId: departmentId.value } };
    }
  } else {
    location = { name: "Orders", params: { type: typeId.value } };
  }
  await Router.push(location);
}

async function getFinanceSourceListAsync() {
  isLoading.value = true;
  const response = await FinanceSourcesApi.getList(sessionStore.year, false, 1, 1000, undefined, "Id", undefined);
  if (response) {
    financeSources.value = [ getDefaultSelectItem(), ...response.data!.collection.map(s => {
      return { text: `${ s.number } - ${ s.name }`, value: s.id } as SelectItem<number>;
    }) ];
  }
  isLoading.value = false;
}

async function useFilterAsync() {
  await getDataAsync();
}

async function getDataAsync() {
  isLoading.value = true;
  const response = await OrdersApi.getList(typeId.value, sessionStore.year, tableData.page, tableData.offset, tableData.filter, tableData.orderBy, tableData.descending, departmentId.value, subDepartmentId.value, financeSourceId.value);
  if (response.success) {
    items.value = response.data!.collection ?? [] as OrderShort[];
    tableData.page = response.data!.page ?? tableData.page;
    tableData.total = response.data!.total ?? 0;
    tableData.spentAmount = response.data!.spentAmount;
    tableData.leftAmount = response.data!.leftAmount;
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

function closeDeleteItemDialog() {
  deleteItemDialog.value = false;
}

function confirmDelete(selectedItem: Order) {
  externalErrors.value = {} as Object<string[]>;
  item.value = selectedItem;
  deleteItemDialog.value = true;
}

async function deleteItemAsync() {
  externalErrors.value = {} as Object<string[]>;
  const response = await OrdersApi.delete(item.value.id);
  if (response.success) {
    item.value = {} as Order;
    deleteItemDialog.value = false;
    await getDataAsync();
    toast.add({ severity: "success", summary: t("toast.success"), life: 3000 });
  } else {
    processError(response.error, (errs) => { externalErrors.value = errs });
  }
}

function getDefaultSelectItem(): SelectItem<number> {
  return { value: 0, text: t("all") };
}

interface OrderTablePagingData extends TablePagingData {
  spentAmount: number;
  leftAmount?: number;
}
</script>

<i18n locale="en">
{
  "orders": "Orders",
  "manageOrders": "Orders",
  "orderDetails": "Order Details",
  "description": "Description",
  "total": "Total",
  "status": "Status",
  "leftAmount": "Left on finance source",
  "type": "Type",
  "financeSource": "Finance Source",
  "all": "-all-"
}
</i18n>

<i18n locale="uk">
{
  "orders": "замовлень",
  "manageOrders": "замовленнями",
  "orderDetails": "Деталі замовлення",
  "description": "Опис",
  "status": "Статус",
  "total": "Всього",
  "leftAmount": "Залишок за темою",
  "type": "Категорія",
  "financeSource": "Тема",
  "all": "-всі-"
}
</i18n>