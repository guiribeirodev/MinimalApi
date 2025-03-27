import { useEffect, useState } from "react";
import { User, NewUser, NewTransaction, Transaction } from "./types";

function App() {
  const [users, setUsers] = useState<User[]>([]);
  const [showUserModal, setShowUserModal] = useState(false);
  const [newUser, setNewUser] = useState<NewUser>({ name: "", age: 0 });
  const [showTransactionModal, setShowTransactionModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [newTransaction, setNewTransaction] = useState<NewTransaction>({
    description: "",
    value: 0,
    operation: "expense",
    user_id: null,
  });

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const response = await fetch(
          "http://127.0.0.1:8000/users/transactions"
        );
        const data = await response.json();
        setUsers(data.users);
      } catch (err) {
        console.error("Error get users:", err);
      }
    };

    fetchUsers();
  }, []);

  const addUser = async () => {
    if (!newUser.name || !newUser.age) {
      alert("Preencha os campos nome e idade.");
      return;
    }
    if (newUser.age < 0 || newUser.age > 150) {
      alert("A idade deve estar entre 0 e 150 anos.");
      return;
    }

    if (!Number.isInteger(newUser.age)) {
      alert("A idade deve ser um número inteiro, ex: 15");
      return;
    }

    try {
      const response = await fetch("http://127.0.0.1:8000/users/", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(newUser),
      });

      const user = await response.json();

      setUsers((prevUsers) => [...prevUsers, { ...user, transactions: [] }]);
      setShowUserModal(false);
      setNewUser({ name: "", age: 0 });
    } catch (error) {
      console.error("Error to add user:", error);
    }
  };

  const deleteUser = async (user_id: number) => {
    try {
      await fetch(`http://127.0.0.1:8000/users/${user_id}`, {
        method: "DELETE",
      });

      setUsers(users.filter((user) => user.id !== user_id));
    } catch (err) {
      console.error("Error to delete user:", err);
    }
  };

  const editUser = async () => {
    if (!newUser.name || !newUser.age) {
      alert("Preencha os campos nome e idade.");
      return;
    }
    if (newUser.age < 0 || newUser.age > 150) {
      alert("A idade deve estar entre 0 e 150 anos.");
      return;
    }

    if (!Number.isInteger(newUser.age)) {
      alert("A idade deve ser um número inteiro, ex: 15");
      return;
    }

    try {
      await fetch(`http://127.0.0.1:8000/users/${newUser.id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name: newUser.name, age: newUser.age }),
      });

      setUsers(
        users.map((user) =>
          user.id === newUser.id
            ? { ...user, name: newUser.name, age: newUser.age }
            : user
        )
      );

      setShowEditModal(false);
      setNewUser({ name: "", age: 0 });
    } catch (err) {
      console.error("Error to edit user:", err);
    }
  };

  const addTransaction = async () => {
    const currentUser = users.find(user => user.id === newTransaction.user_id);

    if (!newTransaction.description || !newTransaction.value){
      alert("Preencha os campos descrição e valor.");
      return;
    }

    if (newTransaction.value < 0){
      alert("O valor precisa ser um número positivo.");
      return;
    }
    if (!currentUser) {
      alert("Usuário não encontrado.");
      return;
    }

    if (currentUser.age < 18 && newTransaction.operation == "income") {
      alert("Usuários menores de 18 anos não podem adicionar receitas.");
      return;
    }

  try {
      const response = await fetch("http://127.0.0.1:8000/transactions/", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(newTransaction),
      });

      if (!response.ok) {
        throw new Error("Error to add Transaction");
      }

      const createdTask = await response.json();

      setUsers((prevUsers) =>
        prevUsers.map((user) =>
          user.id === newTransaction.user_id
            ? { ...user, transactions: [...user.transactions, createdTask] }
            : user
        )
      );

      setShowTransactionModal(false);
      setNewTransaction({
        description: "",
        value: 0,
        operation: "expense",
        user_id: null,
      });
    } catch (error) {
      console.error("Error to add Transaction:", error);
    }
  };

  const deleteTransaction = async (transaction_id: number) => {
    try {
      await fetch(`http://127.0.0.1:8000/transactions/${transaction_id}`, {
        method: "DELETE",
      });

      setUsers((prevUsers) =>
        prevUsers.map((user) => ({
          ...user,
          transactions: user.transactions.filter(
            (t: any) => t.id !== transaction_id
          ),
        }))
      );
    } catch (err) {
      console.error("Error to delete Transaction:", err);
    }
  };

  const totalPeople = users.length;
  const { totalExpenses, totalIncomes } = users.reduce(
    (acc, user) => {
      user.transactions.forEach(transaction => {
        if (transaction.operation === "expense") {
          acc.totalExpenses += Number(transaction.value);
        } else {
          acc.totalIncomes += Number(transaction.value);
        }
      });
      return acc;
    },
    { totalExpenses: 0, totalIncomes: 0 }
  );
  
  const netBalance = totalIncomes - totalExpenses;

  return (
    <div className="flex flex-col items-center p-6 min-h-screen">
      <h1 className="text-6xl text-white font-bold mb-6">MAXIPROD</h1>
      <button
        className="mb-4 px-4 py-2 bg-green-500 text-white rounded-lg hover:bg-green-600"
        onClick={() => setShowUserModal(true)}
      >
        Adicionar Pessoa
      </button>

      <div className="w-full max-w-4xl space-y-3">
        {users.map((user) => {


      const { totalUserExpenses, totalUserIncomes } = user.transactions?.reduce(
        (acc, t: Transaction) => {
          if (t.operation === "expense") {
            acc.totalUserExpenses += Number(t.value);
          } else if (t.operation === "income") {
            acc.totalUserIncomes += Number(t.value);
          }
          return acc;
        },
        { totalUserExpenses: 0, totalUserIncomes: 0 }
      )

          const totalUser: number = totalUserIncomes - totalUserExpenses;

          return (
            <div
              key={user.id}
              className="bg-white rounded-lg p-4 hover:bg-blue-100 transition"
            >
              <div className="flex flex-col sm:flex-row items-center justify-between">
                <div>
                  <p className="text-lg font-semibold text-gray-800">
                    {user.name}{" "}
                    <span className="text-sm font-extralight pl-1.5">
                      id: {user.id}
                    </span>
                  </p>
                  <p className="text-gray-600">Idade: {user.age} anos</p>
                </div>

                <div>
                  <button
                    onClick={() => {
                      setNewUser({
                        id: user.id,
                        name: user.name,
                        age: user.age,
                      });
                      setShowEditModal(true);
                    }}
                    className="bg-yellow-500 text-white px-1 py-1 rounded-lg hover:bg-yellow-600 "
                  >
                    📃 Editar
                  </button>
                  <button
                    onClick={() => deleteUser(user.id)}
                    className="ml-2 bg-red-500 text-white px-2 py-1 rounded-lg hover:bg-red-600"
                  >
                    🗑️ Excluir
                  </button>
                </div>
              </div>

              <div className="mt-7">
                <div className="flex items-center justify-between">
                  <h2 className="text-md font-semibold text-gray-700">
                    Transações:
                  </h2>
                  <button
                    onClick={() => {
                      setNewTransaction({
                        ...newTransaction,
                        user_id: user.id,
                      });
                      setShowTransactionModal(true);
                    }}
                    className="px-0.5 py-2 text-sm sm:px-1 sm:py-2.5 bg-green-500 text-white rounded-lg hover:bg-green-600"
                  >
                    ✅ Criar Transação
                  </button>
                </div>

                {user.transactions.length > 0 ? (
                  <ul className="mt-3 space-y-2">
                    {user.transactions.map((transaction: any) => (
                      <li
                        key={transaction.id}
                        className={`p-2 rounded-md ${
                          transaction.operation === "expense"
                            ? "bg-red-100"
                            : "bg-green-100"
                        }`}
                      >
                        <div className="font-bold flex justify-between">
                          <p>{transaction.description}</p>
                          <button
                            onClick={() => deleteTransaction(transaction.id)}
                            className="bg-gray-300 px-2 py-1 text-sm rounded hover:bg-red-400"
                          >
                            ❌ Excluir
                          </button>
                        </div>
                        <p
                          className={`font-medium ${
                            transaction.operation === "expense"
                              ? "text-red-600"
                              : "text-green-600"
                          }`}
                        >
                          R$ {Number(transaction.value).toFixed(2)}
                        </p>
                      </li>
                    ))}
                  </ul>
                ) : (
                  <p className="text-gray-500">Nenhuma transação registrada.</p>
                )}

                <div className="mt-2">
                  <p className="text-md font-semibold text-gray-700">
                    Saldo:{" "}
                    <span
                      className={`${
                        totalUser > 0 ? "text-green-600" : "text-red-600"
                      }`}
                    >
                      R$ {totalUser.toFixed(2)}
                    </span>
                  </p>
                </div>
              </div>
            </div>
          );
        })}
      </div>

      <div className="w-full max-w-4xl space-y-3 bg-white rounded-lg p-4 mt-4">
        <h2 className="text-xl font-bold text-gray-800">Resumo Geral</h2>
        <div className="flex justify-between items-end">
          <div>
            <p className="text-gray-700 font-medium">
              Total de Pessoas: {totalPeople}
            </p>
            <p className="text-red-600 font-medium">
              Total de Gastos: R$ {totalExpenses.toFixed(2)}
            </p>
            <p className="text-green-600 font-medium">
              Total de Receitas: R$ {totalIncomes.toFixed(2)}
            </p>
          </div>
          <div>
            <p
              className={`font-bold ${
                netBalance >= 0 ? "text-green-700" : "text-red-700"
              }`}
            >
              Saldo Líquido: R$ {netBalance.toFixed(2)}
            </p>
          </div>
        </div>
      </div>

      {showUserModal && (
        <div className="fixed inset-0 flex items-center justify-center bg-gray-900">
          <div className="bg-white p-8 rounded-lg">
            <h2 className="text-3xl font-bold mb-5">Adicionar Pessoa</h2>
            <input
              type="text"
              placeholder="Nome"
              value={newUser.name}
              className="border rounded p-2 w-full mb-2"
              onChange={(event) =>
                setNewUser({ ...newUser, name: event.target.value })
              }
            />

            <input
              type="number"
              placeholder="Idade"
              value={newUser.age}
              className="border rounded p-2 w-full mb-4"
              onChange={(event) =>
                setNewUser({ ...newUser, age: parseInt(event.target.value)})
              }
            />
            <button
              className="bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600"
              onClick={addUser}
            >
              Salvar
            </button>
            <button
              className="ml-2 bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600"
              onClick={() => setShowUserModal(false)}
            >
              Cancelar
            </button>
          </div>
        </div>
      )}

      {showTransactionModal && (
        <div className="fixed inset-0 flex items-center justify-center bg-gray-900">
          <div className="bg-white p-8 rounded-lg w-4/5 max-w-3xl">
            <h2 className="text-3xl font-bold mb-5">Criar Transação</h2>
            <p className="text-m">Digite uma Descrição:</p>
            <input
              type="text"
              placeholder="Descrição"
              value={newTransaction.description}
              onChange={(event) =>
                setNewTransaction({
                  ...newTransaction,
                  description: event.target.value,
                })
              }
              className="border p-2 w-full mb-2"
            />

            <p className="text-m mt-2">Digite um Valor:</p>
            <input
              type="number"
              placeholder="Valor"
              value={newTransaction.value}
              onChange={(event) =>
                setNewTransaction({
                  ...newTransaction,
                  value: Number(event.target.value),
                })
              }
              className="border p-2 w-full mb-2"
            />

            <p className="text-m mt-2">Selecione a categoria:</p>
            <select
              value={newTransaction.operation}
              onChange={(event) =>
                setNewTransaction({
                  ...newTransaction,
                  operation: event.target.value as 'expense' | 'income',
                })
              }
              className="border p-2 w-full mb-10"
            >
              <option value="expense">Gasto</option>
              <option value="income">Receita</option>
            </select>
            <button
              onClick={addTransaction}
              className="bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600"
            >
              Salvar
            </button>
            <button
              onClick={() => setShowTransactionModal(false)}
              className="ml-2 bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600"
            >
              Cancelar
            </button>
          </div>
        </div>
      )}

      {showEditModal && (
        <div className="fixed inset-0 flex items-center justify-center bg-gray-900 bg-opacity-50">
          <div className="bg-white p-6 rounded-lg shadow-lg">
            <h2 className="text-xl font-bold mb-4">Editar Usuário</h2>
            <input
              type="text"
              placeholder="Nome"
              value={newUser.name}
              onChange={(event) => setNewUser({ ...newUser, name: event.target.value })}
              className="border p-2 w-full mb-2"
            />
            <input
              type="number"
              placeholder="Idade"
              value={newUser.age}
              onChange={(event) => setNewUser({ ...newUser, age: parseInt(event.target.value) })}
              className="border p-2 w-full mb-2"
            />
            <button
              onClick={editUser}
              className="bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600"
            >
              Salvar
            </button>
            <button
              onClick={() => setShowEditModal(false)}
              className="ml-2 bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600"
            >
              Cancelar
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

export default App;
