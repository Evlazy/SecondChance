export function parseJwt(token: string){
    try{
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            window
                .atob(base64)
                .split('')
                .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );
        return JSON.parse(jsonPayload);
    }catch (e){
        return null;
    }
}

export function IsAdminUser(): boolean{
    const token = sessionStorage.getItem('token');
    if(!token) return false;

    const payload = parseJwt(token);

    const roles = payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || payload?.role;

    if(Array.isArray(roles)){
        return roles.includes('Admin');
    }
    return roles === 'Admin';
}

export interface AuthenticatedUser{
    id: string;
    email: string;
    isAdmin: boolean;
}

export const getAuthenticatedUser = (): AuthenticatedUser | null => {
    const token = sessionStorage.getItem("token");
    if(!token) return null;

    const payload = parseJwt(token);
    if(!payload){
        sessionStorage.removeItem("token");
        return null;
    }

    const currentTime = Date.now()/ 1000;
    if(payload.exp && payload.exp < currentTime){
        sessionStorage.removeItem("token");
        return null;
    }

    const userId = payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] 
        || payload?.nameid 
        || payload?.sub;

    const email = payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] 
        || payload?.email;

    return{
        id: userId,
        email: email || "",
        isAdmin: IsAdminUser(),
    };
};