<template>
  <Loader v-if="loading"></Loader>
  <div v-else class="grid">
    <div class="col-12">
      <div class="card">
        <div class="p-fluid formgrid grid">
          <div class="field col-12">
            <h4>{{ header }}</h4>
          </div>
          <div class="field col-12">
            <Message v-for="err of externalErrors['']" :severity="'error'" :key="err" :closable="false">{{
                err
              }}
            </Message>
          </div>

          <ErrorWrap class="field col-12 md:col-4" :errors="v$.firstName.$errors"
                     :external-errors="externalErrors['FirstName']">
            <label for="firstName">{{ t('firstName') }}</label>
            <InputText id="firstName" type="text" v-model.trim="model.firstName"/>
          </ErrorWrap>
          <ErrorWrap class="field col-12 md:col-4" :errors="v$.middleName.$errors"
                     :external-errors="externalErrors['MiddleName']">
            <label for="middleName">{{ t('middleName') }}</label>
            <InputText id="middleName" type="text" v-model.trim="model.middleName"/>
          </ErrorWrap>
          <ErrorWrap class="field col-12 md:col-4" :errors="v$.lastName.$errors"
                     :external-errors="externalErrors['LastName']">
            <label for="lastName">{{ t('lastName') }}</label>
            <InputText id="lastName" type="text" v-model.trim="model.lastName"/>
          </ErrorWrap>

          <ErrorWrap class="field col-12 md:col-4" :errors="v$.email.$errors"
                     :external-errors="externalErrors['Email']">
            <label for="email">{{ t('email') }}</label>
            <InputText id="email" type="text" v-model.trim="model.email"/>
          </ErrorWrap>
          <ErrorWrap class="field col-12 md:col-4" :errors="v$.primaryPhone.$errors"
                     :external-errors="externalErrors['PrimaryPhone']">
            <label for="primaryPhone">{{ t('primaryPhone') }}</label>
            <InputText id="primaryPhone" type="text" v-model.trim="model.primaryPhone"/>
          </ErrorWrap>
          <ErrorWrap class="field col-12 md:col-4" :errors="v$.reservePhone.$errors"
                     :external-errors="externalErrors['ReservePhone']">
            <label for="reservePhone">{{ t('reservePhone') }}</label>
            <InputText id="reservePhone" type="text" v-model.trim="model.reservePhone"/>
          </ErrorWrap>

          <div class="field col-12">
            <h5>{{ t('organizationRelation') }}</h5>
          </div>
          <ErrorWrap class="field col-12 md:col-4" :errors="v$.departmentId.$errors"
                     :external-errors="externalErrors['DepartmentId']">
            <label for="departmentId">{{ t('department') }}</label>
            <DepartmentSelector id="departmentId"
                                :defaultOptions="departments"
                                v-model="model.departmentId"
                                :includeEmpty="false"></DepartmentSelector>
          </ErrorWrap>
          <ErrorWrap class="field col-12 md:col-4" :errors="v$.subDepartmentId.$errors"
                     :external-errors="externalErrors['SubDepartmentId']">
            <label for="subDepartmentId">{{ t('subDepartment') }}</label>
            <SubDepartmentSelector id="subDepartmentId"
                                   :defaultOptions="subDepartments"
                                   v-model="model.subDepartmentId"
                                   :includeEmpty="true"
                                   :empty-text="t('selectSubDepartment')"
                                   :department-id="model.departmentId"></SubDepartmentSelector>
          </ErrorWrap>
          <ErrorWrap class="field col-12 md:col-4" :errors="v$.role.$errors" :external-errors="externalErrors['Roles']">
            <label for="role">{{ t('role') }}</label>
            <Dropdown id="role" v-model="model.role" :options="roles" optionLabel="text" optionValue="value"></Dropdown>
          </ErrorWrap>

          <template v-if="model.id < 1">
            <div class="field col-12">
              <h5>{{ t('loginData') }}</h5>
            </div>
            <ErrorWrap class="field col-12 md:col-4" :errors="v$.userName.$errors"
                       :external-errors="externalErrors['UserName']">
              <label for="userName">{{ t('userName') }}</label>
              <InputText id="userName" type="text" v-model.trim="model.userName"/>
            </ErrorWrap>
            <ErrorWrap class="field col-12 md:col-4" :errors="v$.password.$errors"
                       :external-errors="externalErrors['Password']">
              <label for="password">{{ t('password') }}</label>
              <PasswordInput id="password" type="text" v-model.trim="model.password" :feedback="true"/>
            </ErrorWrap>
            <ErrorWrap class="field col-12 md:col-4" :errors="v$.passwordRepeat.$errors" :external-errors="[]">
              <label for="passwordRepeat">{{ t('passwordRepeat') }}</label>
              <PasswordInput id="passwordRepeat" type="text" v-model.trim="model.passwordRepeat" :feedback="true"
                             :promptLabel="t('passwordRepeat')"/>
            </ErrorWrap>
          </template>
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
import { email, maxLength, minLength, numeric, required, requiredIf, sameAs } from "@/i18n/validators";
import { SelectItem } from "@/utils/fetch-utils";
import useVuelidate from "@vuelidate/core";
import { ref, onMounted, computed } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";
import { useRolesStore } from "@/store/roles";
import { useSessionStore } from "@/store/session";
import EmployeesApi, { Employee } from "@/services/api/employees";
import ErrorWrap from "@/components/ErrorWrap.vue";
import Loader from "@/components/Loader.vue";
import PasswordInput from "@/components/PasswordInput.vue";
import DepartmentSelector from "@/components/DepartmentSelector.vue";
import SubDepartmentSelector from "@/components/SubDepartmentSelector.vue";

const { t, d } = useI18n({ useScope: "local" });
const route = useRoute();
const Router = useRouter();
const { user } = useSessionStore();
const { roles } = useRolesStore();

const externalErrors = ref({} as Object<string[]>);
const departments = ref([] as SelectItem<number>[]);
const subDepartments = ref([] as SelectItem<number>[]);
const header = ref("");
const loading = ref(false);
const model = ref({ id: 0, departmentId: 0, subDepartmentId: 0, password: "", passwordRepeat: "" } as EmployeeModel);

const isCreateMode = computed(() => {
  return model.value.id < 1;
});

const rules = computed(() => {
  return {
    firstName: { required, maxLength: maxLength(100) },
    middleName: { maxLength: maxLength(100) },
    lastName: { required, maxLength: maxLength(100) },
    email: { required, email, maxLength: maxLength(100) },
    primaryPhone: { required, numeric, maxLength: maxLength(30) },
    reservePhone: { numeric, maxLength: maxLength(30) },
    userName: { requiredIf: requiredIf(isCreateMode), maxLength: maxLength(100) },
    password: { requiredIf: requiredIf(isCreateMode), minLength: minLength(8), maxLength: maxLength(100) },
    passwordRepeat: {
      requiredIf: requiredIf(isCreateMode),
      sameAs: sameAs(model.value.password, t("password")),
    },
    departmentId: { required, numeric },
    subDepartmentId: { required, numeric },
    role: { required, numeric },
  };
});

const v$ = useVuelidate(rules, model, { $lazy: true });

onMounted(async () => {
  loading.value = true;
  let loaded = false;
  if (route.path.endsWith("new")) {
    header.value = t("createEmployee");
    model.value = getDefaultModel();
    loaded = true;
  } else {
    header.value = t("editEmployee");
    const employeeId = parseInt(route.params.employeeId.toString());
    const response = await EmployeesApi.getById(employeeId);
    if (response.success) {
      model.value = mapToModel(response.data!);
      loaded = true;
    }
  }
  if (loaded) {
    departments.value = [ { text: model.value.department, value: model.value.departmentId } ];
    subDepartments.value = [ { text: model.value.subDepartment, value: model.value.subDepartmentId } ];
    loading.value = false;
  } else {
    await Router.push({ name: "NotFound" });
  }
});


function getDefaultModel(): EmployeeModel {
  return {
    id: 0,
    firstName: "",
    middleName: "",
    lastName: "",
    email: "",
    primaryPhone: "",
    reservePhone: "",
    userName: "",
    password: "",
    passwordRepeat: "",
    departmentId: user!.departmentId,
    department: user!.department,
    subDepartmentId: user!.subDepartmentId,
    subDepartment: user!.subDepartment,
    role: roles[roles.length - 1].value,
    editedDate: undefined,
    editor: undefined,
  };
}

function mapToModel(e: Employee): EmployeeModel {
  return {
    id: e.id,
    firstName: e.firstName,
    middleName: e.middleName,
    lastName: e.lastName,
    email: e.email,
    primaryPhone: e.primaryPhone,
    reservePhone: e.reservePhone,
    userName: "",
    password: "",
    passwordRepeat: "",
    departmentId: e.departmentId,
    department: e.department,
    subDepartmentId: e.subDepartmentId,
    subDepartment: e.subDepartment,
    role: e.roles[0],
    editor: e.editor,
    editedDate: e.editedDate,
  };
}

async function saveAsync() {
  externalErrors.value = {} as Object<string[]>;
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) {
    return;
  }
  loading.value = true;
  const response = isCreateMode.value ? await createAsync() : await updateAsync();
  if (response.success) {
    await Router.push({ name: "Employees" });
  } else {
    if (response.error?.errors) {
      externalErrors.value = response.error.errors;
    }
    loading.value = false;
  }
}

function createAsync() {
  return EmployeesApi.create({
    firstName: model.value.firstName,
    middleName: model.value.middleName,
    lastName: model.value.lastName,
    email: model.value.email,
    userName: model.value.userName,
    password: model.value.password,
    primaryPhone: model.value.primaryPhone,
    reservePhone: model.value.reservePhone,
    roles: [ model.value.role ],
    subDepartmentId: model.value.subDepartmentId,
  });
}

function updateAsync() {
  return EmployeesApi.update({
    id: model.value.id,
    firstName: model.value.firstName,
    middleName: model.value.middleName,
    lastName: model.value.lastName,
    email: model.value.email,
    primaryPhone: model.value.primaryPhone,
    reservePhone: model.value.reservePhone,
    roles: [ model.value.role ],
    subDepartmentId: model.value.subDepartmentId,
  });
}

interface EmployeeModel {
  id: number;
  firstName: string;
  middleName: string | undefined;
  lastName: string | undefined;
  email: string;
  primaryPhone: string;
  reservePhone: string | undefined;
  userName: string;
  password: string;
  passwordRepeat: string;
  departmentId: number;
  department: string;
  subDepartmentId: number;
  subDepartment: string;
  role: number;
  editedDate: Date | undefined,
  editor: string | undefined
}
</script>

<i18n locale="en">
{
  "createEmployee": "Create new employee",
  "editEmployee": "Edit employee",
  "viewEmployee": "View employee details",
  "firstName": "First Name",
  "middleName": "Patronymic Name",
  "lastName": "Last Name",
  "email": "Email",
  "primaryPhone": "Primary Phone",
  "reservePhone": "Reserve Phone",
  "userName": "User Name",
  "password": "Password",
  "passwordRepeat": "Repeat password",
  "loginData": "Login data",
  "organizationRelation": "Relation to organization",
  "department": "Department",
  "subDepartment": "Sub-department",
  "role": "Role",
  "lastEdit": "Edited",
  "selectSubDepartment": "Select Sub-department"
}
</i18n>

<i18n locale="uk">
{
  "createEmployee": "Створення нового працівника",
  "editEmployee": "Редагування працівника",
  "viewEmployee": "Перегляд інформації про працівника",
  "firstName": "Ім'я",
  "middleName": "По батькові",
  "lastName": "Прізвище",
  "email": "Електронна пошта",
  "primaryPhone": "Основний телефон",
  "reservePhone": "Резервний телефон",
  "userName": "Ім'я користувача",
  "password": "Пароль",
  "passwordRepeat": "Підтвердіть пароль",
  "loginData": "Дані входу",
  "organizationRelation": "Належність до організації",
  "department": "Відділ",
  "subDepartment": "Підрозділ",
  "role": "Роль",
  "lastEdit": "Редаговано",
  "selectSubDepartment": "Виберіть підрозділ"
}
</i18n>