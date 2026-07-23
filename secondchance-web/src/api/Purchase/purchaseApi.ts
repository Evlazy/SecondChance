import axiosClient from "../axiosClient";

export interface IOrderStatus {
  readonly Pending: "Pending";
  readonly Paid: "Paid";
  readonly Cancelled: "Cancelled";
}

export const OrderStatus: IOrderStatus = {
  Pending: "Pending",
  Paid: "Paid",
  Cancelled: "Cancelled",
};

export type OrderStatus = IOrderStatus[keyof IOrderStatus];

export interface Order{
    id: string;
    buyerId: string;
    sellerId: string;
    productId: string;
    productName: string;
    price: number;
    quantity: number;
    status: OrderStatus;
    createdAt: string;
}

export interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T | null;
    errors: string[] | null;
}

export const purchaseApi = {
    // getMyPurchases: async (): Promise<Order[]> => {
    //     const response = await axiosClient.get<ApiResponse<Order[]>>('/Purchase/my-purchases');

    //     if(response.data?.success){
    //         return response.data || [];
    //     }
    //     return [];
    // },

    getMyPurchases: async (): Promise<Order[]> => {

    const response = await axiosClient.get<Order[]>('/purchase/my-purchases');
    
    return response.data || [];
  },

    getMySales: async (): Promise<Order[]> => {
        const response = await axiosClient.get<Order[]>('/Purchase/my-sales');

        return  response.data || [];
    },

    confirmPayment: async (orderId: string): Promise<ApiResponse<null>> => {
    const response = await axiosClient.post<ApiResponse<null>>(`/Purchase/orders/${orderId}/confirm-payment`);
    return response.data;
  },

  cancelOrder: async (orderId: string): Promise<ApiResponse<null>> => {
    const response = await axiosClient.post<ApiResponse<null>>(`/Purchase/orders/${orderId}/cancel`);
    return response.data;
  }
}

