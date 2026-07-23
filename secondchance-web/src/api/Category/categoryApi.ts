import axiosClient from "../axiosClient";

export interface Categories{
    id: string;
    name: string;
    description: string;
}

export const categoryApi = {
    getAllCategories: async(): Promise<Categories[]> => {
        const response = await axiosClient.get<any>('/Product/all-categories');
        return response.data || [];
    }
}
