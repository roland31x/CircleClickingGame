﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CircleClickingGame
{
    /// <summary>
    /// Interaction logic for testwindow.xaml
    /// </summary>
    public partial class testwindow : Window
    {
        public testwindow()
        {
            NameScope.SetNameScope(this, new NameScope());
            InitializeComponent();
            SliderTest.tw = this;
            //string line = "235,189,226817,2,0,B|249:87,1,101.200001930237,0|8,1:2|0:2,0:2:0:0:";
            //string line = "326,67,644721,6,0,B|330:66|359:60|373:64|346:45|357:75|386:83|394:52|389:38|393:55|410:76|432:86|445:84|446:80|440:74|434:67|432:66|438:74|454:92|474:115|496:131|511:137|515:133|509:125|496:118|480:119|472:127|472:141|484:157|505:170|528:181|545:188|555:192|551:198|537:206|519:218|498:230|484:244|478:253|481:257|492:257|507:253|521:248|533:244|537:244|536:252|527:267|513:287|496:311|476:335|457:356|440:372|426:380|415:379|409:371|407:356|410:339|417:321|424:307|437:297|461:295|478:301|469:313|456:333|441:358|421:384|393:411|361:433|326:449|291:454|260:450|235:434|217:407|209:374|209:336|216:295|229:256|244:224|257:201|268:189|272:192|271:205|261:230|249:261|236:295|223:328|215:358|211:380|215:394|225:398|240:394|259:382|282:367|304:350|326:332|346:317|362:307|374:300|381:299|383:300|383:305|379:311|374:317|369:321|362:325|357:327|354:325|351:321|353:317|357:312|362:308|369:304|377:300|383:299|386:297|394:296|401:295|406:295|410:295|413:295|414:295|415:292|417:283|418:276|418:269|419:264|421:259|422:252|424:248|425:244|426:241|426:238|425:237|424:237|422:237|421:236|419:234|418:234|418:233|419:232|421:230|424:230|426:230|430:229|433:229|437:228|438:226|440:225|440:222|438:220|437:214|433:209|430:202|426:194|424:186|422:178|421:169|421:161|422:154|422:147|424:143|424:139|422:137|419:135|512:135|470:134|452:134|432:133|413:129|395:125|381:118|366:109|354:99|343:87|334:75|327:63|320:52|316:44|314:39|311:38|311:39|311:40|311:52|312:67|314:86|316:106|318:126|319:145|319:161|320:173|320:178|319:178|319:173|318:159|316:142|314:119|312:94|310:67|308:42|306:17|306:0|307:-10|308:-15|308:-11|307:-2|306:15|303:39|299:67|294:96|287:127|280:158|271:189|261:213|251:228|240:236|228:234|217:226|208:210|199:188|191:162|185:133|181:103|180:76|181:51|184:30|191:15|197:5|207:3|219:7|237:16|249:32|260:51|271:74|279:96|284:121|288:145|288:166|284:185|278:201|267:213|253:221|237:224|219:224|199:220|177:212|156:202|136:190|117:177|101:163|87:149|78:134|73:122|73:110|78:101|89:94|105:88|129:86|144:86|144:87|169:92|200:98|229:106|256:115|280:126|302:137|318:149|328:159|336:171|339:182|338:192|334:200|327:208|318:212|307:214|295:216|283:214|271:209|260:204|251:194|243:184|236:171|232:158|228:145|227:130|227:117|229:103|232:92|235:80|239:71|243:68|247:70|251:75|255:83|259:95|263:111|266:129|268:149|271:170|274:192|275:212|276:230|279:248|280:261|283:269|284:275|287:275|288:271|291:261|294:248|296:232|299:213|302:192|304:171|307:153|308:137|311:125|312:119|314:121|314:131|315:158|315:226|315:250|315:233|314:232|314:237|314:244|312:250|312:256|312:260|312:261|314:259|315:254|318:248|320:240|323:229|327:220|332:209|338:200|342:192|347:185|353:180|357:175|361:174|362:173|363:174|361:177|355:180|340:184|328:188|314:192|295:194|272:198|245:200|217:202|188:205|161:208|142:210|144:212|168:214|168:217|145:220|137:224|145:228|161:232|177:236|196:241|213:245|231:249|248:253|266:254|282:256|298:254|311:252|323:248|332:241|339:234|343:225|343:217|338:210|328:202|315:196|298:189|276:184|252:180|225:178|203:178|185:178|169:181|153:185|140:192|128:198|114:206|97:221|78:228|69:233|73:236|83:234|98:230|114:222|132:210|148:194|162:175|184:153|196:131|205:109|212:90|217:74|221:64|224:62|224:67|224:80|221:102|219:126|215:153|209:178|204:202|199:225|192:245|185:263|180:279|174:293|169:304|164:313|160:320|157:324|156:327|152:324|149:320|149:313|150:307|153:297|157:288|162:279|170:269|180:261|189:252|200:244|211:236|221:228|232:220|240:208|248:205|253:206|257:209|259:212|256:214|251:218|244:221|233:226|220:230|204:237|187:244|169:250|150:257|133:265|116:275|101:283|81:292|67:297|58:303|51:308|50:313|53:320|58:327|67:335|78:344|98:364|121:386|145:407|168:426|189:443|208:458|225:467|240:474|248:469|253:458|255:439|252:417|245:388|236:360|224:319|208:276|191:232|172:189|152:146|130:75|109:-47|87:7|69:62|51:106|37:143|26:174|19:202|18:228|19:252|26:275|37:295|53:313|71:329|91:343|116:355|138:363|161:367|181:370|199:367|212:362|220:351|223:335|221:316|216:296|200:273|180:249|154:224|128:200|99:175|73:154|49:134|29:117|14:103|6:92|6:87|14:84|30:86|53:91|81:101|114:113|150:127|188:143|224:161|256:178|282:196|299:212|308:226|307:238|295:246|274:245|243:248|204:248|161:241|117:229|73:214|31:198|-4:182|-33:167|-53:154|-64:145|-68:143|-68:153|-60:167|-44:185|-21:204|4:224|33:241|61:257|89:269|118:273|142:275|153:273|165:268|173:261|173:252|168:241|157:230|141:220|122:209|101:200|78:192|55:186|35:181|15:178|0:178|-11:180|-17:181|-19:182|-15:186|-4:192|14:192|38:188|59:184|69:178|63:173|49:167|27:162|2:159|-25:159|-53:163|-79:171|-99:182|-114:197|-122:214|-124:233|-120:253|-110:273|-92:289|-70:301|-44:308|-13:307|16:299|49:281|81:257|110:225|136:188|156:146|170:103|177:63|177:27|169:0|154:-18|132:-22|105:-12|74:11|41:47|7:94|-24:146|-43:201|-64:253|-80:296|-88:324|-88:338|-75:331|-52:308|-23:268|12:216|51:158|93:99|134:46|174:3|200:-28|216:-44|227:-40|228:-19|221:19|208:68|191:126|169:182|152:230|138:277|126:311|118:328|112:328|108:311|102:279|98:234|93:181|87:125|81:70|65:20|59:-20|63:-50|70:-65|81:-65|90:-50|97:-23|93:13|77:56|55:103|37:147|27:186|25:214|29:232|37:234|50:221|67:194|85:155|102:107|116:55|124:3|126:-39|122:-69|113:-81|98:-74|81:-46|62:-3|43:46|25:94|11:131|4:155|8:166|25:165|51:153|85:133|121:107|154:76|178:43|188:8|180:-27|153:-60|109:-87|54:-106|-5:-113|-60:-105|-100:-83|-118:-50|-110:-7|-74:40|-12:86|63:123|146:147|223:162|280:153|308:130|303:94|264:51|199:9|118:-22|42:-39|-16:-38|-43:-20|-19:1|37:36|114:74|188:107|244:127|271:133|267:125|233:113|185:110|134:114|98:141|83:174|91:196|117:189|148:153|176:96|193:42|204:8|211:8|217:31|220:58|215:70|197:60|170:44|148:39|146:55|172:76|212:76|243:47|249:19|232:22|209:51|203:75|223:55|257:11|266:24|232:48|240:38|261:23|260:23,1,2464.30407550541,14|0,0:0|2:0,2:0:0:0:";
            //string line = "300,35,272092,2,0,P|332:45|363:80,1,59.375,0|0,0:0|0:0,0:0:0:0:";
            string line = "243,139,377020,6,0,B|207:122|173:134|173:134|172:125|172:125|143:135|130:166|130:166|119:191|79:208|79:208|87:214|87:214|43:236|-5:200,1,300,14|4,0:0|0:0,0:0:0:0:";
            string[] properties = line.Split(',');
            int x = int.Parse(properties[0]);
            int y = int.Parse(properties[1]);
            int time = int.Parse(properties[2]);
            int type = int.Parse(properties[3]);
            string[] pars = new string[3];
            if ((type & 2) > 0)
            {
                pars[0] = properties[5]; // curve pts
                pars[1] = properties[6]; // slides 
                pars[2] = properties[7]; // length
            }
            SliderTest test = new SliderTest(x,y,pars);
           // new ClickableSlider(x, y, pars).Spawn();
            //Test1(0);
            //MessageBox.Show("1");
            //Test1(100);
            //MessageBox.Show("2");
        }
    }
    class SliderTest
    {
        // 243,139,377020,6,0,B|207:122|173:134|173:134|172:125|172:125|143:135|130:166|130:166|119:191|79:208|79:208|87:214|87:214|43:236|-5:200,1,300,14|4,0:0|0:0,0:0:0:0:
        public static testwindow tw;
        public Rectangle tomove;
        public Path Body { get; set; }
        public Ellipse End { get; set; }
        public SliderTest(int x, int y, string[] pars)
        {
            PathFigure pathFigure = new PathFigure();

            Point sp = new Point(x, y);
            pathFigure.StartPoint = sp;


            string[] curvepts = pars[0].Split('|');
            //PolyLineSegment slidersegment = new PolyLineSegment();
            
            int c = 1;
            bool done = false;
            Point? Last = null;
            List<Point> pts = new List<Point>();
            while (c < curvepts.Length && !done)
            {
                pts = new List<Point>();
                if(c == 1)
                {
                    pts.Add(sp);
                    Last = sp;
                }
                for (int i = c; i < curvepts.Length; i++)
                {
                    double xc = double.Parse(curvepts[i].Split(':')[0].Trim());
                    double yc = double.Parse(curvepts[i].Split(':')[1].Trim());
                    Point toAdd = new Point(xc, yc);

                    if(Last != null && toAdd == Last)
                    {
                        c = i;
                        Last = null;
                        break;                       
                    }
                    if(i == curvepts.Length - 1)
                    {
                        done = true;
                    }
                    pts.Add(toAdd);
                    Last = toAdd;
                    
                }
                Point[] Curve = new Point[pts.Count];
                for(int i = 0; i < pts.Count; i++)
                {
                    Curve[i] = pts[i];
                }
                bool ellipsedrew = false;
                for(double t = 0.01; t <= 1; t+= 0.01)
                {
                    if(done && !ellipsedrew)
                    {
                        Point px = beziercoord(Curve, 1);
                        End = new Ellipse()
                        {
                            Height = 50,
                            Width = 50,
                            Fill = new SolidColorBrush(Colors.Black),
                        };
                        tw.MainCanvas.Children.Add(End);
                        Canvas.SetTop(End, px.Y - 25);
                        Canvas.SetLeft(End, px.X - 25);
                        ellipsedrew = true;
                    }
                    LineSegment slidersegment = new LineSegment()
                    {
                        Point = beziercoord(Curve, t),
                    };
                    pathFigure.Segments.Add(slidersegment);
                }
            }
                

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection
            {
                pathFigure
            };

            Path path = new Path()
            {
                Data = pathGeometry,
                StrokeThickness = 2,
                Stroke = new SolidColorBrush(Colors.Black),
            };

            Body = path;

            Rectangle aRectangle = new Rectangle();
            aRectangle.Width = 30;
            aRectangle.Height = 30;
            aRectangle.Fill = Brushes.Blue;
            tomove = aRectangle;
            tw.MainCanvas.Children.Add(path);
            tw.MainCanvas.Children.Add(aRectangle);
            TranslateTransform animatedTranslateTransform = new TranslateTransform();
            aRectangle.RenderTransform = animatedTranslateTransform;
            DoubleAnimationUsingPath translateXAnimation = new DoubleAnimationUsingPath();
            translateXAnimation.PathGeometry = pathGeometry;
            translateXAnimation.Duration = TimeSpan.FromSeconds(5);
            translateXAnimation.Source = PathAnimationSource.X;
            Storyboard.SetTarget(translateXAnimation, aRectangle);
            Storyboard.SetTargetProperty(translateXAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));

            DoubleAnimationUsingPath translateYAnimation = new DoubleAnimationUsingPath();
            translateYAnimation.PathGeometry = pathGeometry;
            translateYAnimation.Duration = TimeSpan.FromSeconds(5);
            translateYAnimation.Source = PathAnimationSource.Y;
            Storyboard.SetTarget(translateYAnimation, aRectangle);
            Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            // Create a Storyboard to contain and apply the animations.
            Storyboard pathAnimationStoryboard = new Storyboard();
            //pathAnimationStoryboard.RepeatBehavior = RepeatBehavior.Forever;
            pathAnimationStoryboard.Children.Add(translateXAnimation);
            pathAnimationStoryboard.Children.Add(translateYAnimation);
            pathAnimationStoryboard.Completed += PathAnimationStoryboard_Completed;
            pathAnimationStoryboard.Begin();
            

        }

        private void PathAnimationStoryboard_Completed(object? sender, EventArgs e)
        {
            Rectangle aRectangle = new Rectangle();
            aRectangle.Width = 30;
            aRectangle.Height = 30;
            aRectangle.Fill = Brushes.Orange;
            tw.MainCanvas.Children.Add(aRectangle);
            Canvas.SetTop(aRectangle, Canvas.GetTop(tomove));
            Canvas.SetLeft(aRectangle, Canvas.GetLeft(tomove));
            //t.MainCanvas.Children.Remove(tomove);
        }

        Point beziercoord(Point[] pointArray, double t)
        {
            double bx = 0, by = 0; 
                int n = pointArray.Length - 1; // degree

            if (n == 1)
            { // if linear
                bx = (1 - t) * pointArray[0].X + t * pointArray[1].X;
                by = (1 - t) * pointArray[0].Y + t * pointArray[1].Y;
            }
            else if (n == 2)
            { // if quadratic
                bx = (1 - t) * (1 - t) * pointArray[0].X + 2 * (1 - t) * t * pointArray[1].X + t * t * pointArray[2].X;
                by = (1 - t) * (1 - t) * pointArray[0].Y + 2 * (1 - t) * t * pointArray[1].Y + t * t * pointArray[2].Y;
            }
            else if (n == 3)
            { // if cubic
                bx = (1 - t) * (1 - t) * (1 - t) * pointArray[0].X + 3 * (1 - t) * (1 - t) * t * pointArray[1].X + 3 * (1 - t) * t * t * pointArray[2].X + t * t * t * pointArray[3].X;
                by = (1 - t) * (1 - t) * (1 - t) * pointArray[0].Y + 3 * (1 - t) * (1 - t) * t * pointArray[1].Y + 3 * (1 - t) * t * t * pointArray[2].Y + t * t * t * pointArray[3].Y;
            }
            else
            { // generalized equation
                for (var i = 0; i <= n; i++)
                {
                    bx += this.binomialCoef(n, i) * Math.Pow(1 - t, n - i) * Math.Pow(t, i) * pointArray[i].X;
                    by += this.binomialCoef(n, i) * Math.Pow(1 - t, n - i) * Math.Pow(t, i) * pointArray[i].Y;
                }
            }

            return new Point(bx, by);
        }

        long binomialCoef(long n, long k)
        {
            long r = 1;

            if (k > n)
                return 0;

            for (var d = 1; d <= k; d++)
            {
                r *= n--;
                r /= d;
            }

            return r;
        }
        static public long Combinations(int n, int k)
        {
            if (k < 0 || k > n)
                return 0;
            if (k == 0 || k == n)
                return 1;
            k = Math.Min(k, n - k);  // take advantage of symmetry
            long c = 1;
            for (int i = 0; i < k; i++)
                c = c * (n - i) / (i + 1);
            return c;
        }
        double Factorial(int n)
        {
            if (n == 0) 
                return 1;
            else
                return n * Factorial(n - 1);
        }
    }
}