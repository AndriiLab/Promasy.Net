import Main from "./views/Main.vue";
import { useSessionStore } from "./store/session";

export const routes = [
  {
    path: "/",
    component: Main,
    children: [
      {
        name: "Dashboard",
        path: "/",
        component: () => import("./pages/Dashboard.vue"),
      },
      {
        name: "CpvList",
        path: "/cpv",
        component: () => import("./pages/Cpv.vue"),
      },
      {
        name: "Units",
        path: "/units",
        component: () => import("./pages/Units.vue"),
      },
      {
        name: "CreateUnit",
        path: "/units/new",
        component: () => import("./pages/Units.vue"),
      },
      {
        name: "Manufacturers",
        path: "/manufacturers",
        component: () => import("./pages/Manufacturers.vue"),
      },
      {
        name: "CreateManufacturer",
        path: "/manufacturers/new",
        component: () => import("./pages/Manufacturers.vue"),
      },
      {
        name: "Suppliers",
        path: "/suppliers",
        component: () => import("./pages/Suppliers.vue"),
      },
      {
        name: "CreateSupplier",
        path: "/suppliers/new",
        component: () => import("./pages/Suppliers.vue"),
      },
      {
        name: "ReasonsForSupplierChoice",
        path: "/reasons-for-supplier-choice",
        component: () => import("./pages/ReasonsForSupplierChoice.vue"),
      },
      {
        name: "CreateReasonForSupplierChoice",
        path: "/reasons-for-supplier-choice/new",
        component: () => import("./pages/ReasonsForSupplierChoice.vue"),
      },
      {
        name: "Finances",
        path: "/finances",
        component: () => import("./pages/Finances.vue"),
      },
      {
        name: "FinanceSubDepartments",
        path: "/finances/:financeId(\\d+)/sub-departments",
        component: () => import("./pages/FinanceSubDepartments.vue"),
      },
      {
        name: "OrdersByCpv",
        path: "/reports/cpv-orders",
        component: () => import("./pages/CpvOrders.vue"),
      },
      {
        name: "Organization",
        path: "/organizations/:organizationId(\\d+)",
        component: () => import("./pages/Organization.vue"),
      },
      {
        name: "Departments",
        path: "/departments",
        component: () => import("./pages/Departments.vue"),
      },
      {
        name: "DepartmentEmployees",
        path: "/departments/:departmentId(\\d+)/employees",
        component: () => import("./pages/Employees.vue"),
      },
      {
        name: "SubDepartments",
        path: "/departments/:departmentId(\\d+)/sub-departments",
        component: () => import("./pages/SubDepartments.vue"),
      },
      {
        name: "SubDepartmentEmployees",
        path: "/departments/:departmentId(\\d+)/sub-departments/:subDepartmentId(\\d+)/employees",
        component: () => import("./pages/Employees.vue"),
      },
      {
        name: "Employees",
        path: "/employees",
        component: () => import("./pages/Employees.vue"),
      }, 
      {
        name: "EmployeeCreate",
        path: "/employees/new",
        component: () => import("./pages/EmployeeView.vue"),
      },
      {
        name: "EmployeeView",
        path: "/employees/:employeeId(\\d+)",
        component: () => import("./pages/EmployeeView.vue"),
      },
      {
        name: "EmployeeMe",
        path: "/me",
        redirect: () => `/employees/${ useSessionStore().user?.id }`,
      },
      {
        name: "Orders",
        path: "/orders/type/:type(\\d+)",
        component: () => import("./pages/Orders.vue"),
      },
      {
        name: "FinanceOrders",
        path: "/finances/:financeId(\\d+)/orders/type/:type(\\d+)",
        component: () => import("./pages/Orders.vue"),
      },
      {
        name: "FinanceDepartmentOrders",
        path: "/finances/:financeId(\\d+)/departments/:departmentId(\\d+)/orders/type/:type(\\d+)",
        component: () => import("./pages/Orders.vue"),
      },
      {
        name: "DepartmentOrders",
        path: "/departments/:departmentId(\\d+)/orders/type/:type(\\d+)",
        component: () => import("./pages/Orders.vue"),
      },
      {
        name: "SubDepartmentsOrders",
        path: "/departments/:departmentId(\\d+)/sub-departments/:subDepartmentId(\\d+)/orders/type/:type(\\d+)",
        component: () => import("./pages/Orders.vue"),
      },
      {
        name: "FinanceSubDepartmentsOrders",
        path: "/finances/:financeId(\\d+)/sub-departments/:subDepartmentId(\\d+)/orders/type/:type(\\d+)",
        component: () => import("./pages/Orders.vue"),
      },
      {
        name: "OrderNew",
        path: "/orders/new",
        component: () => import("./pages/OrderView.vue"),
      },
      {
        name: "OrderView",
        path: "/orders/:orderId(\\d+)",
        component: () => import("./pages/OrderView.vue"),
      },
    ],
  },
  {
    name: "Login",
    path: "/login",
    component: () => import("./views/Login.vue"),
  },
  {
    name: "Logout",
    path: "/logout",
    component: () => import("./views/Logout.vue"),
  },
  {
    name: "NotFound",
    path: "/404",
    component: () => import("./views/NotFound.vue"),
  },
  {
    name: "AccessDenied",
    path: "/403",
    component: () => import("./views/AccessDenied.vue"),
  },
  {
    name: "Error",
    path: "/error",
    component: () => import("./views/Error.vue"),
  },
  {
    path: "/:pathMatch(.*)*",
    redirect: "/404",
  },
];

export const publicPages = [ "/login" ];
export const errorPages = [ "/404", "/403", "/error" ];
