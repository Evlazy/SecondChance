import { getAuthenticatedUser } from "../../utils/jwtHelper";
import React, {useEffect, useState, useTransition} from "react";
import { purchaseApi, type Order, OrderStatus } from "../../api/Purchase/purchaseApi";

type TabType = "purchases" | "sales";

export const MyOrders: React.FC = () =>  {
    const [orders, setOrders] = useState<Order[]>([]);
    const [activeTab, setActiveTab] = useState<TabType>("purchases");
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [errorMsg, setErrorMsg] = useState<string | null>(null);
    const [actioningOrderId, setActioningOrderId] = useState<string | null>(null);

    const [isPending, startTransition] = useTransition();

    const currentUser = getAuthenticatedUser();

const fetchOrders = async (tab: TabType) => {
    setIsLoading(true);
    setErrorMsg(null);
    try {        
        const response = tab === "purchases"
            ? await purchaseApi.getMyPurchases() 
            : await purchaseApi.getMySales();
            
        let finalOrdersArray: any[] = [];
        if (Array.isArray(response)) {
            finalOrdersArray = response;
        } else if (response && Array.isArray((response as any).data)) {
            finalOrdersArray = (response as any).data;
        }

        setOrders(finalOrdersArray);

    } catch (err: any) {
        setErrorMsg("Failed to load your orders. Please try again later.");
    } finally {
        setIsLoading(false);
    }
};

    useEffect(() => {
        if(!currentUser){
            window.location.href = "/login";
            return;
        }
        fetchOrders(activeTab);
    }, [activeTab]);

    const handleTabChange = (tab: TabType) => {
        startTransition(() => {
            setActiveTab(tab);
        });
    };

    const handleCancelOrder = async (orderId: string) => {
        if(!window.confirm("Are you sure you want to cancel this order? This will restore the product.")){
            return;
        }

        setActioningOrderId(orderId);
        setErrorMsg(null);

        try{
            await purchaseApi.cancelOrder(orderId);
            await fetchOrders(activeTab);
        }catch(err: any){
            setErrorMsg(err.response?.data?.message || "Could not cancel the order.");

        }finally{
            setActioningOrderId(null);
        }
    };

    const handleConfirmPayment = async (orderId: string) => {
        setActioningOrderId(orderId);
        setErrorMsg(null);

        try{
            await purchaseApi.confirmPayment(orderId);
            await fetchOrders(activeTab);

        }catch(err: any){
            setErrorMsg(err.response?.data?.message || "Payment confirmation failed.");
        } finally{
            setActioningOrderId(null);
        }
    };

    const getStatusBadgeStyle = (status: OrderStatus) => {
        switch (status) {
            case OrderStatus.Paid:
                return { backgroundColor: "#e6f4ea", color: "#137333", border: "1px solid #137333" };
            case OrderStatus.Cancelled:
                return { backgroundColor: "#f1f3f4", color: "#5f6368", border: "1px solid #dadce0" };
            case OrderStatus.Pending:
            default:
                return { backgroundColor: "#fef7e0", color: "#b06000", border: "1px solid #b06000" };
        }
    };

return (
    <div style={styles.container}>
      <header style={styles.header}>
        <h1 style={styles.title}>Order Management</h1>
        <p style={styles.subtitle}>Track your acquisitions and sales securely.</p>
      </header>

      {/* Tabs */}
      <div style={styles.tabsContainer}>
        <button
          onClick={() => handleTabChange("purchases")}
          style={{
            ...styles.tabButton,
            ...(activeTab === "purchases" ? styles.activeTab : {}),
          }}
          disabled={isPending}
        >
          My Purchases
        </button>
        <button
          onClick={() => handleTabChange("sales")}
          style={{
            ...styles.tabButton,
            ...(activeTab === "sales" ? styles.activeTab : {}),
          }}
          disabled={isPending}
        >
          My Sales
        </button>
      </div>

      {/* Error Banner */}
      {errorMsg && <div style={styles.errorBanner}>{errorMsg}</div>}

      {/* Loading State */}
      {isLoading ? (
        <div style={styles.loader}>Loading secure transaction ledger...</div>
      ) : orders.length === 0 ? (
        <div style={styles.emptyState}>No transaction records found for this section.</div>
      ) : (
        <div style={styles.ordersGrid}>
          {orders.map((order) => {
            console.log("Raw order payload from C#:", order);
            const isBuyer = order.buyerId === currentUser?.id;
            const isPendingPayment = order.status === OrderStatus.Pending;

            return (
              <div key={order.id} style={styles.orderCard}>
                <div style={styles.cardHeader}>
                  <span style={styles.productName}>{order.productName}</span>
                  <span style={{ ...styles.badge, ...getStatusBadgeStyle(order.status) }}>
                    {order.status}
                  </span>
                </div>

                <div style={styles.cardBody}>
                  <div style={styles.dataRow}>
                    <span style={styles.label}>Order ID:</span>
                    <span style={styles.valueCode}>{order.id}</span>
                  </div>
                  <div style={styles.dataRow}>
                    <span style={styles.label}>Amount:</span>
                    <span style={styles.price}>
                      {new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" }).format(order.price)}
                    </span>
                  </div>
                  <div style={styles.dataRow}>
                    <span style={styles.label}>Quantity:</span>
                    <span style={styles.value}>{order.quantity}</span>
                  </div>
                  <div style={styles.dataRow}>
                    <span style={styles.label}>Created:</span>
                    <span style={styles.value}>
                      {new Date(order.createdAt).toLocaleString()}
                    </span>
                  </div>
                </div>

                {/* 🛡️ Secure Authority check directly on the UI level */}
                {isPendingPayment && isBuyer && activeTab === "purchases" && (
                  <div style={styles.actionContainer}>
                    <button
                      onClick={() => handleConfirmPayment(order.id)}
                      disabled={actioningOrderId !== null}
                      style={{ ...styles.button, ...styles.payButton }}
                    >
                      {actioningOrderId === order.id ? "Processing..." : "Confirm Payment"}
                    </button>
                    <button
                      onClick={() => handleCancelOrder(order.id)}
                      disabled={actioningOrderId !== null}
                      style={{ ...styles.button, ...styles.cancelButton }}
                    >
                      Cancel Order
                    </button>
                  </div>
                )}
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
};

const styles: Record<string, React.CSSProperties> = {
  container: {
    maxWidth: "1200px",
    margin: "0 auto",
    padding: "2rem 1.5rem",
    fontFamily: "system-ui, -apple-system, sans-serif",
  },
  header: {
    marginBottom: "2rem",
  },
  title: {
    fontSize: "2rem",
    fontWeight: "700",
    color: "#1a1a1a",
    margin: "0 0 0.5rem 0",
  },
  subtitle: {
    color: "#666",
    margin: 0,
  },
  tabsContainer: {
    display: "flex",
    gap: "0.5rem",
    borderBottom: "2px solid #eaeaea",
    marginBottom: "2rem",
  },
  tabButton: {
    padding: "0.75rem 1.5rem",
    fontSize: "1rem",
    fontWeight: "600",
    background: "none",
    border: "none",
    borderBottom: "3px solid transparent",
    cursor: "pointer",
    color: "#666",
    transition: "all 0.2s ease",
  },
  activeTab: {
    color: "#0066cc",
    borderBottomColor: "#0066cc",
  },
  errorBanner: {
    backgroundColor: "#fce8e6",
    color: "#c5221f",
    padding: "1rem",
    borderRadius: "8px",
    border: "1px solid #fad2cf",
    marginBottom: "1.5rem",
    fontWeight: "500",
  },
  loader: {
    textAlign: "center",
    padding: "3rem",
    color: "#666",
    fontSize: "1.1rem",
  },
  emptyState: {
    textAlign: "center",
    padding: "4rem 2rem",
    backgroundColor: "#f9f9f9",
    borderRadius: "12px",
    border: "1px dashed #ccc",
    color: "#666",
  },
  ordersGrid: {
    display: "grid",
    gridTemplateColumns: "repeat(auto-fill, minmax(350px, 1fr))",
    gap: "1.5rem",
  },
  orderCard: {
    backgroundColor: "#fff",
    borderRadius: "12px",
    border: "1px solid #eaeaea",
    boxShadow: "0 2px 8px rgba(0,0,0,0.05)",
    padding: "1.5rem",
    display: "flex",
    flexDirection: "column",
    justifyContent: "space-between",
  },
  cardHeader: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "flex-start",
    marginBottom: "1.25rem",
    gap: "0.5rem",
  },
  productName: {
    fontSize: "1.2rem",
    fontWeight: "600",
    color: "#1a1a1a",
  },
  badge: {
    padding: "0.25rem 0.75rem",
    borderRadius: "50px",
    fontSize: "0.85rem",
    fontWeight: "600",
    textTransform: "uppercase",
  },
  cardBody: {
    display: "flex",
    flexDirection: "column",
    gap: "0.75rem",
    marginBottom: "1.5rem",
  },
  dataRow: {
    display: "flex",
    justifyContent: "space-between",
    fontSize: "0.95rem",
  },
  label: {
    color: "#666",
  },
  value: {
    color: "#1a1a1a",
    fontWeight: "500",
  },
  valueCode: {
    fontFamily: "monospace",
    color: "#666",
    fontSize: "0.85rem",
  },
  price: {
    fontWeight: "600",
    color: "#1a1a1a",
  },
  actionContainer: {
    display: "flex",
    gap: "0.75rem",
    marginTop: "auto",
  },
  button: {
    flex: 1,
    padding: "0.75rem",
    borderRadius: "8px",
    fontWeight: "600",
    fontSize: "0.95rem",
    border: "none",
    cursor: "pointer",
    transition: "all 0.2s",
  },
  payButton: {
    backgroundColor: "#0066cc",
    color: "#fff",
  },
  cancelButton: {
    backgroundColor: "#fff",
    color: "#d93025",
    border: "1px solid #d93025",
  },
};