
 public static class Binary_Operation_Utils {
   
   
   public static Bool_Object Obtain_Value( this Binary_Operation expr, Context context ) {

    var pair= expr.Left.Validate( context, false ) ;
    if(  !pair.Bool && pair.Object==null ) return new Bool_Object( false, null );
    if( pair.Object is bool || pair.Object is string ) {

      Operation_System.Print_in_Console( "Entre objetos de tipo string o bool no se pueden realizar operaciones de multiplicacion o division ");
      return new Bool_Object( false, null );
    }
    double optr = (double)(pair.Object) ;
    return Obtain_Value( optr, expr.Op, expr.Right, context );

   }


   public static Bool_Object Obtain_Value( double acum, string optr, Expr_Or_Stat expr, Context context ) {
    
     var obj= ( expr.Is_Product() ) ? ((Binary_Operation)expr).Left.Validate( context, false).Object : expr.Validate( context, false).Object ;
     if( obj==null ) return new Bool_Object( false, null);
     if( obj is bool || obj is string ) {

      Operation_System.Print_in_Console( "Las operaciones de producto o division no pueden ser efectuadas entre strings o booleanos" );
      return new Bool_Object( false, null);
     }

     double op= (double)obj;
     double temp= 0;
     switch(optr) {
      case "*":
      temp= acum*op ;
      break;
      case "/":
      temp= acum/op ;
      break;
      case "%":
      temp= acum%op ;
      break;
     }

    if( !expr.Is_Product() ) return new Bool_Object( true, temp );
     
     Binary_Operation aux= (Binary_Operation)expr ;
     return Obtain_Value( temp, aux.Op, aux.Right, context );

   }

   public static bool Is_Product( this Expr_Or_Stat expr ) {

    if ( !(expr is Binary_Operation) )  return false;
    string op= ((Binary_Operation)expr).Op ; 
    return op=="*"  || op=="/" || op=="%" ;

   }

   public static Bool_Object Oposite_Of( this Bool_Object pair ) {

    if( !pair.Bool || pair.Object==null || !(pair.Object is bool) ) {
      
      if( !(pair.Object is bool ) ) Operation_System.Print_in_Console( "Semantik Error: La operacion de negacion solo se aplica a valores booleanos" ) ;
      return new Bool_Object( false, null );
    }
    return new Bool_Object( true, !((bool)pair.Object) ) ;

   }


  
 }

  