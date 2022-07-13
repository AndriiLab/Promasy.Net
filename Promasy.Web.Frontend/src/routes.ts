import Main from "./views/Main.vue";
import { useSessionStore } from "./store/session";

export const routes = [
  {
    path: "/",
    component: Main,
    children: [
      {
        path: "/",
        component: () => import("./pages/Dashboard.vue"),
      },
      {
        path: "/orders",
        component: () => import("./pages/Orders.vue"),
      },
      {
        path: "/cpv",
        component: () => import("./pages/Cpv.vue"),
      },
      {
        path: "/units",
        component: () => import("./pages/Units.vue"),
      },
      {
        path: "/manufacturers",
        component: () => import("./pages/Manufacturers.vue"),
      },
      {
        path: "/suppliers",
        component: () => import("./pages/Suppliers.vue"),
      },
      {
        path: "/finances",
        component: () => import("./pages/Finances.vue"),
      },
      {
        path: "/cpv-orders",
        component: () => import("./pages/CpvOrders.vue"),
      },
      {
        path: "/organizations/:organizationId(\\d+)",
        component: () => import("./pages/Organization.vue"),
      },
      {
        path: "/departments",
        component: () => import("./pages/Departments.vue"),
      },
      {
        path: "/sub-departments",
        component: () => import("./pages/SubDepartments.vue"),
      },
      {
        path: "/employees",
        component: () => import("./pages/Employees.vue"),
      },      {
        path: "/employees/new",
        component: () => import("./pages/EmployeeView.vue"),
      },
      {
        path: "/employees/:employeeId(\\d+)",
        component: () => import("./pages/EmployeeView.vue"),
      },
      {
        path: "/me",
        redirect: () => `/employees/${ useSessionStore().user?.id }`,
      },
    ],
  },
  {
    path: "/login",
    component: () => import("./views/Login.vue"),
  },
  {
    path: "/logout",
    component: () => import("./views/Logout.vue"),
  },
  {
    path: "/404",
    component: () => import("./views/NotFound.vue"),
  },
  {
    path: "/403",
    component: () => import("./views/AccessDenied.vue"),
  },
  {
    path: "/error",
    component: () => import("./views/Error.vue"),
  },
];

export const publicPages = [ "/login" ];
export const errorPages = [ "/404", "/403", "/error" ];
