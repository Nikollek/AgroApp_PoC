namespace AgroApp_PoC
{
    internal class Program
    {

        //instancia classe de conexao ao banco de dados
        static AcessoDB acessoDB = new AcessoDB();

        static void Main(string[] args)
        {
            //opção de ser fornecedor ou cliente ao usuario
            Console.Write("Para ser Cliente digite 1, para ser Fornecedor digite 2: ");
            string escolha = Console.ReadLine();

            if(escolha == "1") {
                //cadastro usuario
                string nome = CadastroUsuario();
                Console.Write("Voce está cadastrado :) ");
                //cadastramento de vendas
                Vendas(nome);

            }else if(escolha == "2"){
                //cadastro fornecedor
                CadastroFornecedor();
                Console.Write("Voce está cadastrado :) ");

            }
        }

        public static void CadastroFornecedor()
        {
            Console.Write("Para pessoa fisica, digite 'f' e para pessoa juridica, digite 'j': ");
            string tipoPessoa = Console.ReadLine();

            if(tipoPessoa == "f")
            {
                Console.Write("CPF: ");
                string cpf = Console.ReadLine();

                Console.Write("Nome: ");
                string nome = Console.ReadLine();

                Console.Write("idade(apenas maiores de 18): ");
                string idade = Console.ReadLine();

                Console.Write("Insira o tipo plantio (Tomate ou Alface): ");
                string alimento = Console.ReadLine();

                acessoDB.CadastroFornecedorPF(tipoPessoa, cpf, nome, idade, alimento);  

            }else if(tipoPessoa == "j")
            {
                Console.Write("CNPJ: ");
                string cnpj = Console.ReadLine();

                Console.Write("Razao social: ");
                string razaoSocial = Console.ReadLine();

                Console.Write("Nome fantasia: ");
                string nomeFantasia = Console.ReadLine();

                Console.Write("telefone: ");
                string telefone = Console.ReadLine();

                Console.Write("Insira o endereço: ");
                Console.Write("Logradouro: ");
                string logradouro = Console.ReadLine();
                Console.Write("numero: ");
                string numero = Console.ReadLine();
                Console.Write("bairro: ");
                string bairro = Console.ReadLine();
                Console.Write("complemento: ");
                string complemento = Console.ReadLine();
                Console.Write("cep: ");
                string cep = Console.ReadLine();
                Console.Write("uf: ");
                string uf = Console.ReadLine();
                Console.Write("municipio: ");
                string municipio = Console.ReadLine();

                Console.Write("Insira o tipo plantio (Tomate ou Alface): ");
                string alimento = Console.ReadLine();

                acessoDB.CadastroFornecedorPJ(tipoPessoa, cnpj, razaoSocial, 
                    nomeFantasia, telefone, logradouro, numero, bairro, complemento, cep, uf, municipio, alimento);
            }

        }

        //Metodo para cadastro de usuario
        public static string CadastroUsuario()
        {
            Console.Write("Digite seu nome: ");
            string nome = Console.ReadLine();

            Console.Write("Digite seu email: ");
            string email = Console.ReadLine();

            Console.Write("Digite sua senha: ");
            string senha = Console.ReadLine();

            //instancia de acesso ao banco com metodo de cadastro usuario
            acessoDB.CadastroUsuario(nome, email, senha);

            //retornando nome do usuario
            return nome;
        }


        //metodo de vendas, tanto de alimentos como de visistas guiadas
        public static void Vendas(string nome)
        {
            bool continuar = true;

            //lop para a possibilidade do usuario comprar a quantidade que quiser
            while(continuar){
                
                Console.Write("Se quiser comprar uma visita digite 1, para comprar alimentos digite 2: ");
                string escolha = Console.ReadLine();

                if (escolha == "1"){
                    //venda visita
                    Console.Write("Quantos ingressos? ");
                    string quantidade = Console.ReadLine();

                    Console.Write("Ingresso solidario, qual o tipo de alimento? ");
                    string alimento = Console.ReadLine();

                    //instancia da classe do banco de dados para venda da visita
                    acessoDB.VendaVisita(nome, quantidade, alimento);

                }else if (escolha == "2"){
                    //venda de alimentos
                    Console.Write("Qual alimento (Tomate ou Alface)? ");
                    string alimento = Console.ReadLine();

                    //instancia da classe do banco de dados para a venda de alimentos
                    string mensagem = acessoDB.VendaAlimentos(nome, alimento);
                    Console.WriteLine(mensagem);

                }else{
                    Console.WriteLine("Opção inválida. Por favor, escolha 1 ou 2.");
                }

                // Perguntar ao usuário se deseja continuar
                Console.Write("Deseja continuar (S/N)? ");
                string continuarStr = Console.ReadLine();

                // Verificar se o usuário deseja continuar
                continuar = (continuarStr.ToLower() == "s");
            }
        }
    }
}