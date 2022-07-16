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
            <label for="department" class="mr-2">{{ t('department') }}</label>
            <DepartmentSelector id="department"
                                v-model="departmentId"
                                :default-options="departments"
                                :include-empty="false">
            </DepartmentSelector>
          </template>
        </Toolbar>

        <DataTable ref="dt" :value="items" :lazy="true" :paginator="true"
                   :rows="tableData.offset" :totalRecords="tableData.total" :loading="isLoading"
                   @page="onPageAsync($event)" @sort="onSortAsync($event)" dataKey="id"
                   paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                   :rowsPerPageOptions="[10,50,100]"
                   :currentPageReportTemplate="t('table.paginationFooter', { itemName: t('subdepartments') })"
                   responsiveLayout="scroll">
          <template #header>
            <div class="flex flex-column md:flex-row md:justify-content-between md:align-items-center">
              <h5 class="m-0">{{ t('table.header', {itemName: t('manageSubdepartments')}) }}</h5>

              <span class="block mt-2 md:mt-0 p-input-icon-left">
                  <i class="pi pi-search"/>
                  <InputText v-model.trim="tableData.filter" v-debounce:500="useFilterAsync"
                             :placeholder="t('table.search')"/>
              </span>
            </div>
          </template>

          <Column field="name" :header="t('name')" :sortable="true" headerStyle="width:35%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('name') }}</span>
              {{ slotProps.data.name }}
            </template>
          </Column>
          <Column field="editor" :header="t('table.columns.editor')" headerStyle="width:25%; min-width:10rem;">
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
          <Column headerStyle="min-width:20rem;">
            <template #body="slotProps">
              <router-link :to="{ name: 'SubDepartmentEmployees', params: { departmentId: slotProps.data.departmentId, subDepartmentId: slotProps.data.id } }">
                <Button v-tooltip.left="t('employees')" icon="pi pi-users" class="p-button-rounded p-button-primary mr-2"/>
              </router-link>
              <Button v-tooltip.left="t('edit')" icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2" @click="edit(slotProps.data)"/>
              <Button v-tooltip.left="t('delete')" icon="pi pi-trash" class="p-button-rounded p-button-warning mt-2"
                      @click="confirmDelete(slotProps.data)"/>
            </template>
          </Column>
        </DataTable>


        <Dialog v-model:visible="itemDialog" :style="{width: '450px'}" :header="t('subdepartmentDetails')" :modal="true"
                class="p-fluid">
          <div class="field">
            <Message v-for="err of externalErrors['']" :severity="'error'" :key="err" :closable="false">{{
                err
              }}
            </Message>
            <div>
              <label for="department">{{ t('department') }}</label>
              <InputText id="department" :value="departments.find(d => d.value === item.departmentId).text" disabled/>
            </div>
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
import { SelectItem } from "@/utils/fetch-utils";
import { capitalize } from "@/utils/string-utils";
import { useSessionStore } from "@/store/session";
import { ref, reactive, onMounted, computed, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useToast } from "primevue/usetoast";
import { useI18n } from "vue-i18n";
import { required, maxLength } from "@/i18n/validators";
import { DataTableSortEvent, DataTablePageEvent } from "primevue/datatable";
import SubDepartmentsApi, { SubDepartment } from "@/services/api/sub-departments";
import DepartmentsApi from "@/services/api/departments";
import ErrorWrap from "../components/ErrorWrap.vue";
import useVuelidate from "@vuelidate/core";
import DepartmentSelector from "@/components/DepartmentSelector.vue";

const Router = useRouter();
const route = useRoute();
const { d, t } = useI18n();
const { user } = useSessionStore();
const organizationId = user!.organizationId;
const departments = ref([ { text: user!.department, value: user!.departmentId } as SelectItem<number> ]);
const departmentId = ref(user!.departmentId);
const toast = useToast();
const items = ref([] as SubDepartment[]);
const externalErrors = ref({} as Object<string[]>);
const item = ref({} as SubDepartment);
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

onMounted(async () => await initAsync());

watch(departmentId, async (id) => {
  await Router.push({ name: "SubDepartments", params: { departmentId: id } });
});

watch(() => route.params.departmentId, async (newId, oldId) => {
  if(oldId !== newId){
    await initAsync();
  }
});

async function initAsync() {
  isLoading.value = true;
  const response = await DepartmentsApi.getById(parseInt(route.params.departmentId.toString()), organizationId);
  if(response.success){
    departments.value = [ { text: response.data!.name, value: response.data!.id } ];
    departmentId.value = response.data!.id;
    await getDataAsync();
    return;
  } else {
    await Router.push({ name: "NotFound" });
  }
}

async function useFilterAsync() {
  await getDataAsync();
}

async function getDataAsync() {
  isLoading.value = true;
  const response = await SubDepartmentsApi.getList(organizationId, departmentId.value, tableData.page, tableData.offset, tableData.filter, tableData.orderBy, tableData.descending);
  if (response.success) {
    items.value = response.data?.collection ?? [] as SubDepartment[];
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
  item.value = { departmentId: departmentId.value } as SubDepartment;
  itemDialog.value = true;
}

function edit(selectedItem: SubDepartment) {
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

function confirmDelete(selectedItem: SubDepartment) {
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
      ? SubDepartmentsApi.update({
        id: item.value.id,
        name: item.value.name,
        organizationId: organizationId,
        departmentId: departmentId.value,
      })
      : SubDepartmentsApi.create({
        name: item.value.name,
        organizationId: organizationId,
        departmentId: departmentId.value,
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
  const response = await SubDepartmentsApi.delete(item.value.id, organizationId, departmentId.value);
  if (response.success) {
    item.value = {} as SubDepartment;
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
  "department": "Department",
  "employees": "Employees",
  "subdepartments": "sub-departments",
  "manageSubdepartments": "sub-departments",
  "name": "Name",
  "subdepartmentDetails": "Sub-Department Details"
}
</i18n>

<i18n locale="uk">
{
  "department": "Відділ",
  "employees": "Працівники",
  "subdepartments": "підрозділів",
  "manageSubdepartments": "підрозділами",
  "name": "Назва",
  "subdepartmentDetails": "Деталі підрозділу"
}
</i18n>