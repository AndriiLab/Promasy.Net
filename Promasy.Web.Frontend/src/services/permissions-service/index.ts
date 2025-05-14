import {PermissionAction} from "@/constants/PermissionActionEnum";
import {PermissionCondition} from "@/constants/PermissionConditionEnum";
import {useSessionStore} from "@/store/session";

export default {
  canAccess(tag: string, action: PermissionAction, params: PermissionParams): boolean {
    const store = useSessionStore();
    const user = store.user;
    if (!user) return false;
    
    const permissions = store.permissions;
    if (!permissions[tag]) return false;
    if (!permissions[tag][action]) return false;

    const condition = permissions[tag][action];
    switch (condition) {
      case PermissionCondition.Allowed:
        return true;
      case PermissionCondition.SameOrganization:
        return params.organizationId === user.organizationId;
      case PermissionCondition.SameDepartment:
        return params.departmentId === user.departmentId;
      case PermissionCondition.SameSubDepartment:
        return params.subDepartmentId === user.subDepartmentId;
      case PermissionCondition.SameUser:
        return params.userId === user.id;
      default:
        return false;
    }
  }
}

export class PermissionParams {
  public userId?: number;
  public subDepartmentId?: number;
  public departmentId?: number;
  public organizationId?: number;
}
