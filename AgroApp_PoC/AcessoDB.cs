using System.Data.SqlClient;

namespace AgroApp_PoC
{
    internal class AcessoDB
    {
        //string de conexão para o banco de dados MS SQL SERVER
        public string connectionString = "Server = NIKOLLE\\SQLEXPRESS; Database = AgroApp; User Id = root; Password = root;";

        //command para receber query a ser executada
        private SqlCommand cmd;

        //valores ficticios de regiao e clima que virão da api de inteligencia artificial
        private string regiao = "Sudeste";
        private string clima = "Tropical";

        private SqlConnection AbrirConexao()
        {
            try
            {
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                return con;
            }
            catch (Exception erro)
            {
                MessageBox.Show(erro.Message);
                return null;
            }
        }

        private void FecharConexao(SqlConnection con)
        {
            con.Close();
        }

        public void CadastroUsuario(string nome, string email, string senha)
        {
            //abertura conexao com o banco de dados
            SqlConnection con = AbrirConexao();

            //Query sql para insercao dos dados na tabela Clientes
            string SQL = "INSERT INTO Clientes (nome, email, senha) VALUES ";
            SQL += "('" + nome + "','" + email + "','" + senha + "')";

          
            cmd = new SqlCommand(SQL, con);
            cmd.ExecuteNonQuery();

            //fechar conexao com o banco de dados
            FecharConexao(con);
        }

        public void VendaVisita(string nome, string quantidade, string alimentoIngresso)
        {
            //abertura conexao com o banco de dados
            SqlConnection con = AbrirConexao();

            //Buscar o id_cliente com o nome dele 
            string SQLSelect = "SELECT id_cliente FROM Clientes WHERE nome = @nome";
            cmd = new(SQLSelect, con);
            cmd.Parameters.AddWithValue("@nome", nome);
            int idCliente = Convert.ToInt32(cmd.ExecuteScalar());

            //Insercao Manual dos dados do VendaVisita
            string total = "30";

            //Query sql para insercao dos dados na tabela VendaVisita com o idCliente inserido no Cadastro
            string SQL = "INSERT INTO VendaVisita (quantidade, total, alimento_ingresso, id_cliente) VALUES ";
            SQL += "('" + quantidade + "','" + total + "','" + alimentoIngresso + "','" + idCliente + "')";

            cmd = new SqlCommand(SQL, con);
            cmd.ExecuteNonQuery();

            //fechar conexao com o banco de dados
            FecharConexao(con);

        }

        public string VendaAlimentos(string nome, string alimento)
        {
            //abertura conexao com o banco de dados
            SqlConnection con = AbrirConexao();

            //query sql para retorno do idPLantacao onde o tipo de alimento escolhido pelo usuario foi fornecido pelo fornecedor e que não esteja vendido
            string SQLBuscaAlimento = "SELECT id_plantacao FROM Plantacao WHERE id_plantios = " +
                "(SELECT id_plantios FROM Plantios WHERE descricao = @descricao) AND id_venda IS NULL";
            cmd = new(SQLBuscaAlimento, con);
            cmd.Parameters.AddWithValue("@descricao", alimento);
            int idPlantacao = Convert.ToInt32(cmd.ExecuteScalar());

            //validacao ver se o Plantacao escolhida pelo cliente está disponivel
            object result = cmd.ExecuteScalar();
            if(result == null)
            {
                return "Não existe esse alimento no nosso estoque";
            }

            //query sql para selecao retorno do idcliente do cliente que esta fazendo a compra
            string SQLBuscaCliente = "SELECT id_cliente FROM Clientes WHERE nome = @nome ";
            cmd = new(SQLBuscaCliente, con);
            cmd.Parameters.AddWithValue("@nome", nome);
            int idCliente = Convert.ToInt32(cmd.ExecuteScalar());

            //insercao manual do valor para a venda
            double total = 30;

            //query sql para a insercao dos dados nas vendas dos alimentos
            string SQLInsercaoVenda = "INSERT INTO VendaAlimento (total, id_cliente) VALUES ";
            SQLInsercaoVenda += "('" + total + "','" + idCliente + "')";

            cmd = new SqlCommand(SQLInsercaoVenda, con);
            cmd.ExecuteNonQuery();

            //query sql para o retorno do id_venda que foi inserido na linha acima
            string SQLBuscaIdVenda = "SELECT id_venda FROM VendaAlimento WHERE id_cliente = @idCliente ";
            cmd = new(SQLBuscaIdVenda, con);
            cmd.Parameters.AddWithValue("@idCliente", idCliente);
            int idVenda = Convert.ToInt32(cmd.ExecuteScalar());

            //query sql para update da tabela Plantacao onde, inserido que eles foram finalizados, preco e id que leva pra venda
            string SQLPlantacaoVendida = "UPDATE Plantacao SET valor_final = @valorFinal " +
                ",finalizado = @finalizado ,id_venda = @idVenda WHERE id_plantacao = @idPlantacao ";
            cmd = new(SQLPlantacaoVendida, con);
            cmd.Parameters.AddWithValue("@valorFinal", total);
            cmd.Parameters.AddWithValue("@finalizado", Boolean.TrueString);
            cmd.Parameters.AddWithValue("@idVenda", idVenda);
            cmd.Parameters.AddWithValue("@idPlantacao", idPlantacao);
            cmd.ExecuteNonQuery();

            //fechar conexao com o banco de dados
            FecharConexao(con);

            return "Venda efetivada :)";


        }

        internal void CadastroFornecedorPF(string tipoPessoa, string cpf, string nome, string idade, string alimento)
        {

            //abertura conexao com o banco de dados
            SqlConnection con = AbrirConexao();

            string SQL = "INSERT INTO PessoaFisica (cpf, nome, idade) VALUES ";
            SQL += "('" + cpf + "','" + nome + "','" + idade + "')";

            cmd = new SqlCommand(SQL, con);
            cmd.ExecuteNonQuery();

            string SQLFornecedor = "INSERT INTO Fornecedores (tipo_pessoa, cpf) VALUES ";
            SQLFornecedor += "('" + tipoPessoa + "','" + cpf + "')";

            cmd = new SqlCommand(SQLFornecedor, con);
            cmd.ExecuteNonQuery();

            //Buscar o id fornecedor da pessoa fisica
            string SQLSelect = "SELECT id_fornecedor FROM Fornecedores WHERE cpf = @cpf";
            cmd = new(SQLSelect, con);
            cmd.Parameters.AddWithValue("@cpf", cpf);
            int idFornecedor = Convert.ToInt32(cmd.ExecuteScalar());

            InsercaoPlantacao(con, alimento, idFornecedor);

            //fechar conexao com o banco de dados
            FecharConexao(con);
        }

        internal void CadastroFornecedorPJ(string tipoPessoa, string cnpj,
            string razaoSocial, string nomeFantasia,
            string telefone, string logradouro,
            string numero, string bairro,
            string complemento, string cep,
            string uf, string municipio, string alimento)
        {
            //abertura conexao com o banco de dados
            SqlConnection con = AbrirConexao();

            string SQL = "INSERT INTO PessoaJuridica (cnpj, razao_social, nome_fantasia, telefone, " +
                "logradouro, numero, bairro, complemento, cep, uf, municipio) VALUES ";
            SQL += "('" + cnpj + "','" + razaoSocial + "','" + nomeFantasia + "','" + telefone + "','"
                + logradouro + "','" + numero + "','" + bairro + "','" + complemento + "','" + cep + "','" + uf + "','" + municipio + "')";

            cmd = new SqlCommand(SQL, con);
            cmd.ExecuteNonQuery();

            string SQLFornecedor = "INSERT INTO Fornecedores (tipo_pessoa, cnpj) VALUES ";
            SQLFornecedor += "('" + tipoPessoa + "','" + cnpj + "')";

            cmd = new SqlCommand(SQLFornecedor, con);
            cmd.ExecuteNonQuery();

            //Buscar o id fornecedor da pessoa fisica
            string SQLSelect = "SELECT id_fornecedor FROM Fornecedores WHERE cnpj = @cnpj";
            cmd = new(SQLSelect, con);
            cmd.Parameters.AddWithValue("@cnpj", cnpj);
            int idFornecedor = Convert.ToInt32(cmd.ExecuteScalar());

            InsercaoPlantacao(con, alimento, idFornecedor);

            //fechar conexao com o banco de dados
            FecharConexao(con);
        }

        private void InsercaoPlantacao(SqlConnection con, string alimento, int idFornecedor)
        {
            //Buscar o id fornecedor da pessoa fisica
            string SQLSelectPlantios = "SELECT id_plantios FROM Plantios WHERE descricao = @descricao";
            cmd = new(SQLSelectPlantios, con);
            cmd.Parameters.AddWithValue("@descricao", alimento);
            int idPlantio = Convert.ToInt32(cmd.ExecuteScalar());

            string SQLPlantacao = "INSERT INTO Plantacao (regiao, clima, id_fornecedor, id_plantios) VALUES ";
            SQLPlantacao += "('" + regiao + "','" + clima + "','" + idFornecedor + "','" + idPlantio + "')";

            cmd = new SqlCommand(SQLPlantacao, con);
            cmd.ExecuteNonQuery();
        }
    }
}
