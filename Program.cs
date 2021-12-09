using Spectre.Console;
using Spectre.Console.Rendering;
using System.Text.Json;

#region Helpers
Rule rule(string text = "")
{
    return string.IsNullOrWhiteSpace(text) ? new Rule() : new Rule(text);
}

SelectionPrompt<T> selection<T>(IEnumerable<T> choices) where T : notnull
{
    return new SelectionPrompt<T>()
        .AddChoices(choices);
}

SelectionPrompt<int> mapSelection(string[] choices)
{
    return selection(choices.Select((_, i) => i))
        .UseConverter(i => choices[i]);
}

Action<string> mkLine = AnsiConsole.MarkupLine;

void writeRend(IRenderable obj)
{
    AnsiConsole.Write(obj);
}

void write(string text)
{
    AnsiConsole.Write(text);
}

void writeLine(string obj = "")
{
    if (obj is null || string.IsNullOrWhiteSpace(obj))
    {
        AnsiConsole.WriteLine();
    }
    else
    {
        AnsiConsole.WriteLine(obj);
    }
}

void clear()
{
    AnsiConsole.Clear();
}

ConsoleKeyInfo readKey()
{
    return Console.ReadKey();
}

T prompt<T>(IPrompt<T> obj)
{
    return AnsiConsole.Prompt(obj);
}

FigletText figlet(string text)
{
    return new FigletText(text);
}

int exitOption<T>(T[] array)
{
    return array.Length - 1;
}
#endregion

var title = figlet("ASDF - O Guia Interativo").Centered().Color(Color.DarkCyan);

var aboutTitle = "O que é o ASDF?";
var installTitle = "Como instalar o ASDF";
var pluginsTitle = "Plugins - O que são e como instalar";

var aboutRule = rule($"[bold cyan]{aboutTitle}[/]").LeftAligned().RuleStyle("green");
var installRule = rule($"[bold cyan]{installTitle}[/]").LeftAligned().RuleStyle("green");
var pluginsRule = rule($"[bold cyan]{pluginsTitle}[/]").LeftAligned().RuleStyle("green");

var choices = new[] {
    $"1) {aboutTitle}",
    $"2) {installTitle}",
    $"3) {pluginsTitle}",
    "*) Sair"
};

var installChoices = new[]
{
    $"1) Dependências",
    $"2) Download",
    $"3) Instalação",
    $"*) Voltar"
};

var pluginChoices = new[]
{
    $"1) O que são Plugins?",
    $"2) Onde encontrar Plugins?",
    $"3) Instalando Plugins",
    $"4) Utilizando Plugins",
    $"*) Voltar"
};

var options = mapSelection(choices);
var installOptions = mapSelection(installChoices);
var pluginOptions = mapSelection(pluginChoices);

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add("user-agent", "request");
var asdfType = new { tag_name = "" };
var asdfData = await httpClient.GetAsync("https://api.github.com/repos/asdf-vm/asdf/releases/latest");
dynamic? json = await JsonSerializer.DeserializeAsync(await asdfData.Content.ReadAsStreamAsync(), asdfType.GetType());

string tag = json?.tag_name ?? "";

int option;

void showAbout()
{
    clear();
    writeRend(aboutRule);
    mkLine("    No início do uso de [bold]linguagens de programação[/] como as que temos hoje, não havia uma forte adoção da [bold]internet[/]. Muitas linguagens, runtimes e SDKs vinham em [bold]disquetes[/], e posteriormente em [bold]CDs[/].");
    mkLine("    Com o passar dos anos e a [bold]adoção da web[/], muitas linguagens passaram a ter seus [bold]instaladores disponibilizados de forma online[/], através de [bold]websites[/] ou mesmo de [bold]gerenciadores de pacotes[/] de sistemas operacionais.");
    mkLine("    Houve, porém, o surgimento de alguns problemas: os gerenciadores de pacotes dos sistemas operacionais, principalmente aqueles que não são rolling release (com atualizações diárias e frequentes), começaram a [bold]manter versões desatualizadas dessas ferramentas[/]. Além disso, a [bold]variedade de versões de cada linguagem[/] fazia com que houvesse uma diferença de versões entre os diversos projetos de software, até mesmo dentro de uma única empresa.");
    mkLine("    A comunidade de desenvolvimento passou então a criar [bold]gerenciadores para as versões[/], mas como eram muitas linguagens, os desenvolvedores precisavam aprender a utilizar cada versionador.");
    mkLine("    O ASDF surge com a premissa de [bold]utilizar plugins para manter a sintaxe unificada[/] e [bold]concentrar as instalações em um local melhor gerenciável[/]. Ele permite que com [bold]poucos comandos[/] seja adicionada uma [bold]nova versão ou ferramenta[/], muitas vezes [bold]em poucos segundos[/].");
    writeRend(rule().RuleStyle("green"));
    writeLine();
    write("Pressione um botão para continuar... ");
    readKey();
}

void showInstall()
{
    int installOption;
    do
    {
        clear();
        writeRend(installRule);
        mkLine("Esta seção é dedicada à instalação do ASDF no seu computador. Lembrete: o ASDF [underline]não funciona no SO Windows[/].");
        writeLine();
        installOption = prompt(installOptions);
        switch (installOption)
        {
            case 0:
                clear();
                writeRend(rule("Dependências"));
                mkLine("O ASDF tem duas dependências:");
                writeLine();
                mkLine(" - [darkorange]git[/]: [underline][link=https://git-scm.com/]https://git-scm.com/[/][/]");
                mkLine(" - [deepskyblue4_1]curl[/]: [underline][link=https://curl.se/]https://curl.se/[/][/]");
                writeLine();
                mkLine("Muitas vezes, esses programas já vêm instalados nas distribuições Linux, mas caso não, ambos estão nos gerenciadores de pacotes de todas as grandes distribuições conhecidas e suas derivadas.");
                mkLine("Para instalar em distribuições baseadas em Debian/Ubuntu, utilize o comando:");
                writeLine();
                mkLine("[invert]sudo apt-get install -y git curl[/]");
                writeLine();
                writeRend(rule());
                write("Pressione um botão para continuar... ");
                readKey();
                break;
            case 1:
                clear();
                writeRend(rule("Download"));
                mkLine("Para baixar o ASDF, basta fazer o clone do repositório do GitHub usando o Git:");
                writeLine();
                mkLine($"[invert]git clone https://github.com/asdf-vm/asdf.git ~/.asdf --branch {tag}[/]");
                writeLine();
                writeRend(rule());
                write("Pressione um botão para continuar... ");
                readKey();
                break;
            case 2:
                clear();
                writeRend(rule("Instalação"));
                mkLine("Para instalar o ASDF, é preciso somente incluir o caminho do repositório clonado do GitHub na variável $PATH do seu terminal:");
                writeLine();
                mkLine(" - [green]Bash[/]");
                mkLine("Coloque as seguintes linhas em seu arquivo .bashrc:");
                writeLine();
                mkLine("[invert]. $HOME/.asdf/asdf.sh[/]");
                mkLine("[invert]. $HOME/.asdf/completions/asdf.bash[/]");
                writeLine();
                mkLine(" - [green]Zsh[/]");
                mkLine("Coloque a seguinte linha em seu arquivo .zshrc:");
                writeLine();
                mkLine("[invert]. $HOME/.asdf/asdf.sh[/]");
                writeLine();
                mkLine("Caso utilize algum framework com o Zsh, como o Oh-My-Zsh ou o Zim, instale os plugins de acordo com o framework para habilitar o autocomplete. Caso contrário, adicione as seguintes linhas ao seu .zshrc:");
                writeLine();
                mkLine("[invert]fpath=(${ASDF_DIR}/completions $fpath)[/]");
                mkLine("[invert]autoload -Uz compinit && compinit[/]");
                writeLine();
                writeRend(rule());
                write("Pressione um botão para continuar... ");
                readKey();
                break;
        }
        clear();
    } while (installOption != exitOption(installChoices));
}

void showPlugins()
{
    int pluginOption;
    do
    {
        clear();
        writeRend(pluginsRule);
        mkLine("Esta seção é dedicada à instalação do ASDF no seu computador. Lembrete: o ASDF [underline]não funciona no SO Windows[/].");
        writeLine();
        pluginOption = prompt(pluginOptions);
        switch (pluginOption)
        {
            case 0:
                clear();
                writeRend(rule("O que são Plugins?"));
                mkLine("Os plugins são os gerenciadores de cada linguagem, e é através deles que se utiliza o ASDF, que não tem muita utilização sem eles. Ao invés de instalar um gerenciador específico de uma linguagem, que instala os binários em outro local e possui uma sintaxe completamente diferente, instala-se um plugin para essa linguagem e mantém a configuração do ambiente centralizada em uma única ferramenta.");
                writeRend(rule());
                write("Pressione um botão para continuar... ");
                readKey();
                break;
            case 1:
                clear();
                writeRend(rule("Onde encontrar Plugins?"));
                mkLine("A lista de plugins está localizada no repositório de plugins. Cada plugin tem seu próprio repositório, e muitos deles são mantidos pela comunidade, e não pela equipe do ASDF. Atualmente, não só linguagens, runtimes e SDKs tem plugins, mas também algumas ferramentas, como alguns bancos de dados, ferramentas de build e automação, editores de texto como o Vim e etc. O repositório de plugins é: ");
                writeLine();
                mkLine("[underline link=https://github.com/asdf-vm/asdf-plugins]https://github.com/asdf-vm/asdf-plugins[/]");
                writeLine();
                writeRend(rule());
                write("Pressione um botão para continuar... ");
                readKey();
                break;
            case 2:
                clear();
                writeRend(rule("Instalando Plugins"));
                mkLine("Para instalar um plugin, é só utilizar o comando [italic]plugin-add[/] passando o nome do plugin e o repositório.");
                writeLine();
                mkLine("[invert]asdf plugin-add <nome-plugin> <repositorio-plugin>[/]");
                writeLine();
                writeRend(rule());
                write("Pressione um botão para continuar... ");
                readKey();
                break;
            case 3:
                clear();
                writeRend(rule("Utilizando Plugins"));
                mkLine("Um plugin, geralmente, funciona como um \"wrapper\" para um gerenciador já existente, que faz uma interface utilizando a sintaxe do ASDF.");
                writeLine();
                mkLine("- Listar versões disponíveis: [invert]asdf list-all <nome-plugin>[/]");
                writeLine();
                mkLine("- Baixar uma versão da ferramenta/linguagem: [invert]asdf install <nome-plugin> <versao>[/]");
                writeLine();
                mkLine("- Definir uma versão para uso global: [invert]asdf global <nome-plugin> <versao>[/]");
                writeLine();
                mkLine("- Definir uma versão para uso em um local específico: [invert]asdf local <nome-plugin> <versao>[/]");
                writeLine();
                writeRend(rule());
                write("Pressione um botão para continuar... ");
                readKey();
                break;
        }
        clear();
    } while (pluginOption != exitOption(pluginChoices));
}

do
{
    writeRend(title);
    option = prompt(options);
    switch (option)
    {
        case 0:
            showAbout();
            break;
        case 1:
            showInstall();
            break;
        case 2:
            showPlugins();
            break;
    }
    clear();
} while (option != exitOption(choices));
