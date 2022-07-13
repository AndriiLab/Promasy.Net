<template>
  <div class="grid">
    <div class="col-12">
      <div class="card">

        <Toolbar class="mb-4">
          <template v-slot:start>
            <div class="my-2">
              <router-link :to="'/employees/new'">
                <Button :label="t('createDialog.addNew')" icon="pi pi-plus" class="p-button-success mr-2"/>
              </router-link>
            </div>
          </template>
          <template v-slot:end>
            <DepartmentSelector :default-options="[{ value: user.departmentId, text: user.department }]"
                                v-model="departmentId"
                                :include-empty="true"
                                :label-classes="['mr-2']"
                                :selector-classes="['mr-4', 'w-23rem']">
            </DepartmentSelector>
            <SubDepartmentSelector :department-id="departmentId"
                                   :default-options="[{ value: user.subDepartmentId, text: user.subDepartment }]"
                                   v-model="subDepartmentId"
                                   :label-classes="['mr-2']"
                                   :selector-classes="['w-20rem']">
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
              <Tag v-for="role in slotProps.data.roles" :key="role" :value="getRoleName(role)"></Tag>
            </template>
          </Column>
          <Column field="department" :header="t('department')"
                  headerStyle="width:15%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('department') }}</span>
              {{ slotProps.data.department }}
            </template>
          </Column>
          <Column field="subDepartment" :header="t('subDepartment')"
                  headerStyle="width:15%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('subDepartment') }}</span>
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
                           :to="`/employees/${slotProps.data.id}`">
                <Button icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2"/>
              </router-link>
              <Button icon="pi pi-trash" class="p-button-rounded p-button-warning mt-2"
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
import { useRolesStore } from "@/store/roles";
import { useSessionStore } from "@/store/session";
import { ref, reactive, onMounted, computed, watch } from "vue";
import EmployeesApi, { EmployeeShort } from "@/services/api/employees";
import { useToast } from "primevue/usetoast";
import { useI18n } from "vue-i18n";
import { DataTableSortEvent, DataTablePageEvent } from "primevue/datatable";
import DepartmentSelector from "@/components/DepartmentSelector.vue";
import SubDepartmentSelector from "@/components/SubDepartmentSelector.vue";
import { debounce } from "vue-debounce";

const { d, t } = useI18n();
const { user } = useSessionStore();
const { getRoleName } = useRolesStore();
const toast = useToast();
const items = ref([] as EmployeeShort[]);
const externalErrors = ref({} as Object<string[]>);
const item = ref({} as EmployeeShort);
const deleteItemDialog = ref(false);
const isLoading = ref(false);
const departmentId = ref(user!.departmentId);
const subDepartmentId = ref(user!.subDepartmentId);
const tableData: TablePagingData = reactive({
  page: 1,
  offset: 10,
  filter: undefined,
  orderBy: undefined,
  descending: undefined,
  total: 0,
});
const sortFields: { [key: string]: string } = {
  "name": "LastName",
};

onMounted(async () => {
  await getDataAsync();
});

watch(departmentId, async () => await debouncedGetDataAsync());
watch(subDepartmentId, async () => await debouncedGetDataAsync());

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
    return;
  }
  if (response.error?.errors) {
    externalErrors.value = response.error.errors;
  }
}
</script>

<i18n locale="en">
{
  "employees": "employees",
  "manageEmployees": "employees",
  "name": "Name",
  "department": "Department",
  "subDepartment": "Sub-department",
  "role": "Role"
}
</i18n>

<i18n locale="uk">
{
  "employees": "співробітників",
  "manageEmployees": "співробітниками",
  "name": "Ім'я",
  "department": "Відділ",
  "subDepartment": "Підрозділ",
  "role": "Роль"
}
</i18n>