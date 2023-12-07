
 public class Program {

  public static void Main() {
  
  Operation_System.Interface() ;
  
  }

  public static void Aux() {

  foreach( var pair in Parser.Follow.dicc )  {
   Console.WriteLine( "Key" + "  " + pair.Key );
   foreach( var s in pair.Value )
    if( s.IsEpsilon ) Console.WriteLine( "epsilon" + "  ");
    else Console.Write( s.Class + "  " );

    Console.WriteLine( "\n");
  }

  }


   public static void Aux_Table() {

     foreach( var n in Data.gramatik.No_Terminals ) 
      foreach( var t in Data.gramatik.Terminals ) {
       Console.Write( "Si tenemos "+ n.Class + " y "+ t.Class + " entonces aplicamos :    ");
       var list= Parser.table.Search( n, t);
       if( list== null ) Console.Write( "null");
       else {

      for( int i=0; i<list.Count; i++) {

       Production p= list[i] ;
       Console.Write( p.Left.Class + " => ");
       foreach( var s in p.Right )
       Console.Write( s.Class + "");  
       
       Console.Write( "\n") ;

       }    
      }

      Console.Write( "\n");

      }
   }


   public static void Print( Node node, int height ) {

    Console.WriteLine( node.Symbol + "   " + height );
    foreach( var tree in node.Children )
    Print( tree, height+1 );

   }

   public static void Aux1() {

   Binary_Operation aux= new Binary_Operation( new Binary_Operation( new Number("5"),"+", new Number("5")), "/", new Binary_Operation( new Number("3"), "/", new Number("4")));
   Binary_Operation expr= new Binary_Operation( new Number("3"), "*", new Binary_Operation( new Number("12"), "/", aux ));
   Context context= new Context();
   Console.WriteLine( expr.Obtain_Value( context ).Object );

   }

  

 }


 //instructions ..
 // let x= 2<3 & ( let x1=4 in (x1+2)< 5 | 3<5) in (x==true) ;    answer: true
 // let x=1, y=2 in ( if((Sum(x,y)>6 & 5>x) | "lala"=="land" ) (let x1=3 in (x1/x)) + (print(2))*2 else true ) ;   aunswer: true 
 //let x=1, y=2 in ( if((Sum(x, y)>6 & 5>x) | "lala"=="lala" ) let x1=3 in (x1/x) + print(2)*2 else true ) ;   answer: 7
 // ( let x="lala" in ( x+"land")) / (print("_movie")) ;
 // if( (Fib(3)<Fib(4) & Fib(5)>Fib(6) ) | "movie"=="movi" ) 5 else 2<3 & 4>5 ;
 //print(let x=2>3 | 2<3 in (x) & 2<2 ) ;  answer: false
 //print( if(2<3) 2==2 else 2>3 | 2==2 ) ;
 //print( ( if(2<3) 2==2 else 2>3 ) | 2==2 ) ;
 //print( 2>4 | ( true & 2>1 )) ;
 //print(true & true ) ;
 //10-( 3*2-6/6-( 10+5-14) )+2 ;  answer: 8
 //10-Sum(10,5)/15;  aunswer: 9
 //let x=(let x1="lala" in ( let x2="_land" in (x1+x2) ) )+ "_dame_tu_cosita" in (x=="lala_land_dame_tu_cosita") ;  answer: true
 //if( 2<3 & ( 2<1 | "audi"=="audi" ) ) (let x=2*3 in (x))/2 else 4;    answer: 3
 //"yo"+Sum("_", "robot");
 //true & 2<Sum(2,1) & ( "lala"=="lala" );
 //let x=2<3 in (x)!= 2<3 & 5>4;
 //"lala"=="lala" & !"bebe"=="b" & ( 2+3>5 | !(2>4 & 3<2)) ;   true
 //let x=2 in (2) + print(2)/if(2<3) 2 else 1;
 //if(2<1 | 2<2 ) 2 else let x1="lets", x2="_get" in ( x1+x2 ) + print("_loud") ;  aunswer: lets_get_loud
 //let x="lala" in ( x+ if(2>3) "_land" else 3 );
 //30*2 + 30* let x=30*2 in (x-60) * let x=2 in (x) + 2 + if(2<3) 3 else 9;
 