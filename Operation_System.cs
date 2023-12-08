


public class Operation_System  {

   static bool error_printed; 

  public static bool Validate_Program( string chain ) {
   
    error_printed= false;
   if( Semantik_Analysis.AST== null ) Semantik_Analysis.AST= new Program_Node() ;
   if( Semantik_Analysis.Context== null ) {
    Semantik_Analysis.Context= new Context() ;
    Semantik_Analysis.Context.Introduce_Functions() ; 
   }

   Semantik_Analysis.Context.Clean_Sintax();

   string aux="";
   var tree= (Program_Node)Semantik_Analysis.AST ;

   for( int i= 0; i< chain.Length; i++) 
    if( chain[i]==';' ||  i== chain.Length-1  ) {
     
     aux= ( chain[i]==';' )? chain.Substring( 0, i ) : chain.Substring( 0, i+1 )  ; 
     //Console.WriteLine( aux) ;
     var t= Obtain_AST( aux ) ; 
     if( !t.Item2 ) return false ; 
     var sub_tree= t.Item1 ;
     Console.WriteLine("AST_Completed");
     
     var pair= sub_tree.Validate( Semantik_Analysis.Context, false ) ;
     if( !pair.Bool) return false ;
     if( i== chain.Length-1 && chain[i]!=';' ) {
     
      Print_in_Console( "Sintax Error :  You must finish the instruction with a ';' caracter") ;
       return false ;
     }
     if( pair.Object!= null ) Print_in_Console( pair.Object );
     tree.lines.Add( sub_tree ) ;
     return true ;

    }
    return true ;
    
  }


  public static Tuple<Expr_Or_Stat, bool> Obtain_AST( string s ) {

   Node node= Parser.Parsing( Lexer.Tokenization( s ) ) ;
    if( node== null ) return Tuple.Create<Expr_Or_Stat, bool>( null, false ) ;
    Expr_Or_Stat sub_tree= Semantik_Analysis.To_AST( node ) ;
    if( sub_tree== null )  return Tuple.Create<Expr_Or_Stat, bool>( null, false )  ;
  
    return Tuple.Create<Expr_Or_Stat, bool>( sub_tree, true );
  } 


  public static void Interface() {
    
    string s= ""; 
    do {

    s= Console.ReadLine() ;
    if( s=="finish") break;
    Validate_Program( s );
    }
    while( s!= "finish");
    
  } 

   public static void Print_in_Console( object obj ) { 
    
    if( !error_printed) Console.WriteLine( obj) ; 
     error_printed= true;
   }


}

