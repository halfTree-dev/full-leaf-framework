using System;
using full_leaf_framework.Physics;

namespace full_leaf_framework.Interact;

public class CollisionTestMenu : Hud {

    private PolygonTest testcase;

    public void GetCollisionStatus() {
        testcase = new PolygonTest();
        var test1 = testcase.preSets[((TestPolygon)hudUnits[0]).showingPolygon];
        test1.Translate(((TestPolygon)hudUnits[0]).drawable.pos);
        var test2 = testcase.preSets[((TestPolygon)hudUnits[1]).showingPolygon];
        test2.Translate(((TestPolygon)hudUnits[1]).drawable.pos);
        var test3 = testcase.testCircle;
        test3.Translate(((TestPolygon)hudUnits[2]).drawable.pos);
        Console.WriteLine("Polygon to Polygon -> " + ShapeManager.IsCollision(test1, test2));
        Console.WriteLine("Polygon1 to Circle -> " + ShapeManager.IsCollision(test1, test3));
        Console.WriteLine("Polygon2 to Circle -> " + ShapeManager.IsCollision(test2, test3));
    }

}