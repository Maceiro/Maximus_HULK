
 public static class Parser {
  

  public static SymbolSet Firsts ;
  public static SymbolSet Follow;
  public static Symbol epsilon;
  public static Table table;
  public static bool processing;
  public static Node Tree;
  public static int index;

  public static Node Parsing( List<Token> tokens ) {
   
   if( !processing ) {
     Pre_Processing() ;
     processing= true ;
   }
   
   return ConstructTree( tokens );
   
  }

  public static void Pre_Processing() {

   Data.Create_Gramatik();
   Calculate_Firsts();
   Calculate_Follow();
   BuildTable();
   //Program.Aux_Table();
   
  }


  public static void Calculate_Firsts() {
  
  var firsts= new SymbolSet();
  epsilon= Data.Epsilon;
   foreach( var t in Data.gramatik.Terminals ) {
  
   var list= new List<Symbol>();
   list.Add( t);
   firsts.Add( t, list );
   }

   foreach( var n in Data.gramatik.No_Terminals ) {

   firsts.Add( n, new List<Symbol>() );
   }

   bool change;
   do {
    
    change= false;
   foreach( var p in Data.gramatik.Productions ) {
   
   Symbol left= p.Left ;
   var right= p.Right;

   if(p.IsEpsilon ) { 
    if( change) firsts[left].Add_Bool( epsilon );
    else change= firsts[left].Add_Bool( epsilon );
    continue;
   }

   bool all_epsilon= true;
   foreach( var s in right ) {
    
    if( change) firsts[left].Add_All( firsts[s] );
     else change= firsts[left].Add_All( firsts[s] );

    if( !firsts[s].Contains(epsilon) ) {
     all_epsilon= false;
     break;
    }

     if( all_epsilon) {
      if( change) firsts[left].Add_Bool( epsilon );
       else change= firsts[left].Add_Bool( epsilon );
     }

    
    }

    }

   }
  while( change );

  Firsts= firsts ;
  }


  public static void BuildTable() {

  var table_aux= new Table();

  foreach( var p in Data.gramatik.Productions ) 
   foreach( var t in Data.gramatik.Terminals ) {
    
   // if( t.Class.Length== 0) continue;
    if( !p.IsEpsilon && Calculate_Firsts_Sufix( p.Right, 0 ).Contains_Bool( t) ) table_aux.Add( p.Left, t, p );
    if( p.IsEpsilon && Follow[ p.Left].Contains_Bool( t) ) table_aux.Add( p.Left, t, p ); 
   
   }

   table= table_aux ;
  
  }


  public static Node ConstructTree( List<Token> tokens ) {
   
   index= 0;
   Node tree= new Node( Data.gramatik.Initial.Class, null) ;
   if( Construct( tree, tokens ) && index==tokens.Count-1 ) return tree ;
   return null;

  }

  public static bool Construct( Node node, List<Token> tokens ) {
    
    //Console.WriteLine( node.Symbol ) ;
    if( node.Is_Terminal() ) {
      
      if( node.Is_Epsilon() ) return true ;
      if( node.Symbol!= tokens[index].Class ) {

        Operation_System.Print_in_Console( string.Format("Sintactic Error: Are you missing a {0} token after {1} token ? The error happen after {2}th caracter", node.Symbol, tokens[index-1].Class, tokens[index].Cursor-1 )  );
        return false ;
      }
      node.Chain=tokens[index].Chain ;
      index++;
      Semantik_Analysis.Context.Introduce_Sintax_Context(node);
      return true ;
    }

    int temp= index ;
    var list= table.Search( new Symbol( node.Symbol ), new Symbol( tokens[index].Class ) ) ;
    if( list== null ) {
     
     if( Is_Expression(node.Symbol) ) {
 
     if( node.Symbol!="list_arg") {

     if(index-1>=0) Operation_System.Print_in_Console( string.Format("Sintactic Error: Expression missing after {0} token. The error happen after {1}th caracter", tokens[index-1].Class, tokens[index-1].Cursor ));
     else Operation_System.Print_in_Console( "Sintactic Error: Expression missing in the begining of the instruction");
    
     }

     else Operation_System.Print_in_Console( string.Format("Sintactic Error: Only IDs can be consider as arguments in a Function_Declaration instruction. The error happen after {0}th caracter", tokens[index-1].Cursor+1 ));
    }
     
     return false;
    }

    for( int i=0; i< list.Count; i++) {

     var right= list[i].Right ;
     bool valid_production= false ;
     for( int j=0; j< right.Count; j++) {

      node.Children.Add( new Node(right[j].Class, node ) ) ;
      if( !Construct( node.Children[j], tokens ) ) break ;
      if( j==right.Count-1) valid_production= true ;
     }

     if( valid_production ) return true ;
     if( Is_Expression( node.Symbol) && index-1>=0 )  Follows( node.Symbol, tokens, Semantik_Analysis.Context );
     node.Children= new List<Node>() ;
     index= temp ;

    }
    
    return false ;
     
  }
   

   public static void Calculate_Follow() {
   
   var follow= new SymbolSet();
   foreach( var n in Data.gramatik.No_Terminals ) 
    follow.Add( n, new List<Symbol>() ) ;
   
   follow[Data.gramatik.Initial].Add( Data.EOF );
   bool change;
   do {
    change= false; 
   foreach( var p in Data.gramatik.Productions ) {

    var left= p.Left ;
    var right= p.Right ;
    
    for( int i= 0; i< right.Count; i++ ) {
      
      if( right[i].IsTerminal ) continue;

      var firsts= Calculate_Firsts_Sufix( right, i+1 );

      if( change) follow[right[i]].Add_All( firsts);
       else change= follow[right[i]].Add_All( firsts);


      if( firsts.Contains_Epsilon() || i== (right.Count-1) ) {
        
        if( change) follow[ right[i] ].Add_All( follow[left]);
         else change= follow[ right[i] ].Add_All( follow[left]);
       
      }

    }

   }
      
   }
    while( change);

    Follow=  follow ;

   }


   public static List<Symbol> Calculate_Firsts_Sufix( List<Symbol> symbols, int ini ) {

    var result= new List<Symbol>() ;
    if( ini>= symbols.Count ) return result;
    bool all_epsilon= true;

    for( int i= ini; i< symbols.Count; i++ ) {

      result.Add_All( Firsts[ symbols[i] ] );
      if( !Firsts[symbols[i]].Contains(epsilon) ) {
        all_epsilon= false;
        break;
      }
    }

    if( all_epsilon ) result.Add( epsilon);

    return result;

   }

   public static string Transform( string s, Stack<Symbol_Node> stack) {

    if( s=="expr" || s=="term" || s=="factor" ) return "Expression" ;
    if( s=="atom" || s=="mol") return "ID, Function_Call, Number or String" ;
    if( s=="condition") return "Condition" ;
    if( s=="list_expr") return "Expression List";
    if( s=="list_arg") return "Argument List" ;
    if( s=="line") return "Expression or Statement";
    if( s=="statement") return "Statement";
    if( s=="list_assignments") return "Assignment List" ;
    if( s=="op") return "Comparison Operator";
    if( s=="aux_expr" || s=="aux_term" || s=="aux_factor" ) {
    var aux1= stack.Pop() ;
    string aux2=aux1.Symbol.Class ;
    return Transform( aux2, stack );
    }
    if(s=="aux_list_arg") return ") or Argument List";
    if(s=="aux_list_expr") return ") or Expression List";
    if(s=="aux_list_assignments") return "token \"in\" or more assignments" ;
    //Modificar estos 3 ultimos para que pidan al usuario agregar comas "
    
    return s;
  
   }


  public static bool Is_Expression( string s ) {

    return s=="boolean_op" || s=="condition" || s=="expr" || s=="term" || s=="factor" || s=="atom" || s=="mol" || s=="list_arg" || s=="list_expr";
 
  }

  public static void Follows( string symbol, List<Token> tokens, Context context ) {
    

   if( symbol=="factor" || symbol=="term" || symbol=="atom" || symbol=="list_arg" ) {
    //Console.WriteLine( symbol);
    //Console.WriteLine( context.Get_Sintax_Context());

    string aux= tokens[index].Class;
    if( aux=="Number" || aux=="string" || aux=="boolean" || aux=="ID" || aux=="let" || aux=="if" || aux=="print") 
      switch( context.Get_Sintax_Context()) {

      case "if":
      Operation_System.Print_in_Console( string.Format("Missing_operator or missing \")\" token after {0} token. The error happen in {1}th caracter", tokens[index-1].Class, tokens[index].Cursor-1 ) );
      break;
      case "else":
      Operation_System.Print_in_Console( string.Format("Missing operator or \"else\" token after {0} token. The error happen in {1}th caracter", tokens[index-1].Class, tokens[index].Cursor-1) );
      break;
      case "let":
      if(aux=="ID") Operation_System.Print_in_Console( string.Format("Missing_operator or \", \" token after {0} token. The error happen in {1}th caracter", tokens[index-1].Class, tokens[index].Cursor-1 ) );
      else Operation_System.Print_in_Console( string.Format("Missing_operator token after {0} token. The error happen in {1}th caracter", tokens[index-1].Class, tokens[index].Cursor-1 ) );
      break;
      case "expression":
      Operation_System.Print_in_Console( string.Format("Missing_operator token after {0} token. The error happen in {1}th caracter", tokens[index-1].Class, tokens[index].Cursor-1 ) );
      break; 
      case "func_call":
      Operation_System.Print_in_Console( string.Format("Missing_operator or missing , token after {0} token. The error happen in {1}th caracter", tokens[index-1].Class, tokens[index].Cursor-1) );
      break;
      case "def_func":
      if(aux=="ID") Operation_System.Print_in_Console( string.Format("Missing \", \" token after {0} token. The error happen in {1}th caracter", tokens[index-1].Class, tokens[index].Cursor-1 ) );
      else Operation_System.Print_in_Console( string.Format("Only IDs can be consider as arguments in a Function_Definition instruction. The error happen in {1}th caracter", tokens[index-1].Class, tokens[index].Cursor ) );
      break;
   
   }

   if( aux=="(") 
    switch( context.Get_Sintax_Context()) {

      case "if":
      Operation_System.Print_in_Console( string.Format("Missing operator-token after {0} token. The error happen in {1}th caracter", tokens[index-1].Class, tokens[index].Cursor-1) );
      break;
      case "else":
      Operation_System.Print_in_Console( string.Format("Missing operator or \"else\" token after {0} token. The error happen in {1}th caracter", tokens[index-1].Class, tokens[index].Cursor-1) );
      break;
      case "let":
      Operation_System.Print_in_Console( string.Format("Missing operator or \"in\" token after {0} token. The error happen in {1}th caracter", tokens[index-1].Class, tokens[index].Cursor-1) );
      break;
      case "func_call":
      Operation_System.Print_in_Console( string.Format("Missing operator or missing \",\" token after {0} token. The error happen in {1}th caracter", tokens[index-1].Class, tokens[index].Cursor-1) );
      break;
      case "expression":
      Operation_System.Print_in_Console( string.Format("Missing_operator token after {0} token. The error happen in {1}th caracter", tokens[index-1].Class, tokens[index].Cursor-1) );
      break;

    }

    if( aux=="+" || aux=="-" || aux=="*" || aux=="/" || aux=="^") 
    switch( context.Get_Sintax_Context()) {

      case "def_func":
      Operation_System.Print_in_Console( string.Format("Operations can't be consider as arguments in a Function_Definition instruction. The error happen in {1}th caracter", tokens[index-1].Class, tokens[index].Cursor ) );
      break;
  
    }

  } 

  }

 }

 public static class Node_Extensions {

  public static bool Check_Errors( this Node node ) {
   
   if( ( node.Symbol=="list_arg" || node.Symbol=="list_expr" ) && node.Children.Count==1 && node.Parent!=null && node.Parent.Children[0].Symbol==",") {
   Operation_System.Print_in_Console( "Syntax Error!! : Expression List or Argument List expected after \",\"") ;
   return true ;
   }
   for(int i= 0; i< node.Children.Count; i++ )
    if( node.Children[i].Check_Errors()) return true ;

    return false ;

  }


 }

