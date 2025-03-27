export interface Transaction {
    id: number;
    description: string;
    value: number;
    operation: "expense" | "income";
  }

  export interface NewTransaction {
    description: string;
    value: number;
    operation: "expense" | "income";
    user_id: number | null;
  }
  
  export interface User {
    id: number;
    name: string;
    age: number;
    transactions: Transaction[];
  }
  
  export interface NewUser {
    id?: number;
    name: string;
    age: number;
  }
  