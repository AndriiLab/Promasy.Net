<template>
  <div class="grid">
    <div class="col-12">
      <div class="card">

        <Toolbar class="mb-4">
          <template v-slot:start>
            <div class="my-2">
              <router-link :to="{ name: 'EmployeeCreate' }">
                <Button :label="t('createDialog.addNew')" icon="pi pi-plus" class="p-button-success mr-2"/>
              </router-link>
            </div>
          </template>
          <template v-slot:end>
            <label for="department" class="mr-2">{{ t('department') }}</label>
            <DepartmentSelector id="department"
                                :default-options="departments"
                                v-model="departmentId"
                                :include-empty="true"
                                class="mr-4 w-23rem">
            </DepartmentSelector>
            <label for="subDepartment" class="mr-2">{{ t('sub-department') }}</label>
            <SubDepartmentSelector id="subDepartment"
                                   :department-id="departmentId"
                                   :default-options="subDepartments"
                                   v-model="subDepartmentId"
                                   :include-empty="true"
                                   class="w-20rem">
            </SubDepartmentSelector>
          </template>
        </Toolbar>

        <DataTable ref="dt" :value="items" :lazy="true" :paginator="true"
                   :rows="tableData.offset" :totalRecords="tableData.total" :loading="isLoading"
                   @page="onPageAsync($event)" @sort="onSortAsync($event)" dataKey="id"
                   paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                   :rowsPerPageOptions="[10,50,100]"
                   :currentPageReportTemplate="t('table.paginationFooter', { itemName: t('employees') })"
                   responsiveLayout="scroll">
          <template #header>
            <div class="flex flex-column md:flex-row md:justify-content-between md:align-items-center">
              <h5 class="m-0">{{ t('table.header', {itemName: t('manageEmployees')}) }}</h5>
              <span class="block mt-2 md:mt-0 p-input-icon-left">
                  <i class="pi pi-search"/>
                  <InputText v-model.trim="tableData.filter" v-debounce:500="getDataAsync"
                             :placeholder="t('table.search')"/>
              </span>
            </div>
          </template>

          <Column field="name" :header="t('name')" :sortable="true" headerStyle="width:15%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('name') }}</span>
              {{ slotProps.data.name }}
            </template>
          </Column>
          <Column field="roles" :header="t('role')" headerStyle="width:15%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('role') }}</span>
              <RoleBadge v-for="role in slotProps.data.roles" :key="role" :role="role"></RoleBadge>
            </template>
          </Column>
          <Column field="department" :header="t('department')"
                  headerStyle="width:15%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('department') }}</span>
              {{ slotProps.data.department }}
            </template>
          </Column>
          <Column field="subDepartment" :header="t('sub-department')"
                  headerStyle="width:15%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('sub-department') }}</span>
              {{ slotProps.data.subDepartment }}
            </template>
          </Column>
          <Column field="editor" :header="t('table.columns.editor')" headerStyle="width:15%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('table.columns.editor') }}</span>
              <Chip :label="slotProps.data.editor" icon="pi pi-user"/>
            </template>
          </Column>
          <Column field="editedDate" :header="t('table.columns.editDate')" headerStyle="width:15%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('table.columns.editDate') }}</span>
              {{ d(new Date(slotProps.data.editedDate), 'long') }}
            </template>
          </Column>
          <Column headerStyle="min-width:10rem;">
            <template #body="slotProps">
              <router-link icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2"
                           :to="{ name: 'EmployeeView', params: {employeeId: slotProps.data.id}}">
                <Button v-tooltip.left="t('edit')" icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2"/>
              </router-link>
              <Button v-tooltip.left="t('delete')" icon="pi pi-trash" class="p-button-rounded p-button-warning mt-2"
                      @click="confirmDelete(slotProps.data)"/>
            </template>
          </Column>
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
import { useSessionStore } from "@/store/session";
import processError from "@/utils/error-response-utils";
import { SelectItem } from "@/utils/fetch-utils";
import { ref, reactive, onMounted, computed, watch } from "vue";
import DepartmentsApi from "@/services/api/departments";
import SubDepartmentsApi from "@/services/api/sub-departments";
import EmployeesApi, { EmployeeShort } from "@/services/api/employees";
import { useToast } from "primevue/usetoast";
import { useI18n } from "vue-i18n";
import { DataTableSortEvent, DataTablePageEvent } from "primevue/datatable";
import DepartmentSelector from "@/components/DepartmentSelector.vue";
import SubDepartmentSelector from "@/components/SubDepartmentSelector.vue";
import RoleBadge from "@/components/RoleBadge.vue";
import { debounce } from "vue-debounce";
import { RouteLocationRaw, useRoute, useRouter } from "vue-router";

const { d, t } = useI18n();
const { user } = useSessionStore();
const toast = useToast();
const Router = useRouter();
const route = useRoute();

const items = ref([] as EmployeeShort[]);
const externalErrors = ref({} as Object<string[]>);
const item = ref({} as EmployeeShort);
const deleteItemDialog = ref(false);
const isLoading = ref(false);
const departmentId = ref(0);
const departments = ref([] as SelectItem<number>[]);
const subDepartmentId = ref(0);
const subDepartments = ref([] as SelectItem<number>[]);
const tableData: TablePagingData = reactive({
  page: 1,
  offset: 10,
  filter: undefined,
  orderBy: undefined,
  descending: undefined,
  total: 0,
});
const sortFields: { [key: string]: string } = {
  "name": "ShortName",
};

onMounted(async () => {
  await initAsync();
});

watch(() => route.params.departmentId, async (newId, oldId) => {
  if (oldId !== newId) {
    await initAsync();
  }
});

watch(() => route.params.subDepartmentId, async (newId, oldId) => {
  if (oldId !== newId) {
    await initAsync();
  }
});

watch(departmentId, async () => await debouncedSetPathAsync());
watch(subDepartmentId, async () => await debouncedSetPathAsync());

async function initAsync() {
  let isSuccess = true;
  isLoading.value = true;
  const selectedDepartmentId = parseInt(route.params.departmentId?.toString());
  if (!isNaN(selectedDepartmentId)) {
    isSuccess = await setDepartmentAsync(selectedDepartmentId)
    const selectedSubDepartmentId = parseInt(route.params.subDepartmentId?.toString());
    if (!isNaN(selectedSubDepartmentId) && isSuccess) {
      isSuccess = await setSubDepartmentAsync(selectedSubDepartmentId);
    }
  }

  if (isSuccess) {
    await getDataAsync();
  } else {
    await Router.push({ name: "NotFound" });
  }
}

const debouncedSetPathAsync =
    debounce(async () => await setPathAsync(), 400);

async function setPathAsync() {
  let location: RouteLocationRaw;
  if (departmentId.value) {
    if (subDepartmentId.value) {
      location = {
        name: "SubDepartmentEmployees",
        params: { departmentId: departmentId.value, subDepartmentId: subDepartmentId.value },
      };
    } else {
      location = { name: "DepartmentEmployees", params: { departmentId: departmentId.value } };
    }
  } else {
    location = { name: "Employees" };
  }
  await Router.push(location);
}

async function setDepartmentAsync(id: number) {
  const response = await DepartmentsApi.getById(id, user!.organizationId);
  if (response.success) {
    departments.value = [ { text: response.data!.name, value: response.data!.id } ];
    departmentId.value = response.data!.id;
  }
  return response.success;
}

async function setSubDepartmentAsync(id: number) {
  const response = await SubDepartmentsApi.getById(id, user!.organizationId, departmentId.value);
  if (response.success) {
    subDepartments.value = [ { text: response.data!.name, value: response.data!.id } ];
    subDepartmentId.value = response.data!.id;
  }
  return response.success;
}

const debouncedGetDataAsync =
    debounce(async () => await getDataAsync(), 400);

async function getDataAsync() {
  isLoading.value = true;
  const response = await EmployeesApi.getList(departmentId.value, subDepartmentId.value, tableData.page, tableData.offset, tableData.filter, tableData.orderBy, tableData.descending);
  if (response.success) {
    items.value = response.data?.collection ?? [] as EmployeeShort[];
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
  tableData.orderBy = sortFields[event.sortField!.toString()];
  tableData.descending = event.sortOrder === 1 ? undefined : true;
  await getDataAsync();
}


function closeDeleteItemDialog() {
  deleteItemDialog.value = false;
}

function confirmDelete(selectedItem: EmployeeShort) {
  externalErrors.value = {} as Object<string[]>;
  item.value = selectedItem;
  deleteItemDialog.value = true;
}

async function deleteItemAsync() {
  externalErrors.value = {} as Object<string[]>;
  const response = await EmployeesApi.delete(item.value.id);
  if (response.success) {
    item.value = {} as EmployeeShort;
    deleteItemDialog.value = false;
    await getDataAsync();
    toast.add({ severity: "success", summary: t("toast.success"), life: 3000 });
  } else {
    processError(response.error, (errs) => { externalErrors.value = errs });
  }
}
</script>

<i18n locale="en">
{
  "employees": "employees",
  "manageEmployees": "employees",
  "name": "Name",
  "role": "Role"
}
</i18n>

<i18n locale="uk">
{
  "employees": "працівників",
  "manageEmployees": "працівниками",
  "name": "Ім'я",
  "role": "Роль"
}
</i18n>