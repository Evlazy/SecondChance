import axiosClient from "../axiosClient";

interface AuthResponse {
  token: string;
  expiration: string;
}

export interface RegisterDto{
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  avatarUrl?: string;
}

export const authApi = {
  login: async (email: string, password: string): Promise<AuthResponse> => {
    const response = await axiosClient.post<AuthResponse>('/auth/login', { 
  Email: email, 
  Password: password
});
    return response.data;
  },

  register: async (registerData: RegisterDto) => {
    const response = await axiosClient.post('/auth/register', registerData);
    return response.data;
  }
};