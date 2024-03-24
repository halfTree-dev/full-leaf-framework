using System;
using full_leaf_framework.Physics;

namespace full_leaf_framework.Interact;

public class CollisionTestMenu : Hud {

    private PolygonTest testcase;

    public InputManager inputManager;

    public void FillCollisionTestMenu(InputManager inputManager) {
        this.inputManager = inputManager;
    }

    public void MoveShapePosition(IHudUnit hudUnit) {
        var polygonTest = (TestPolygon)hudUnit;
        if (inputManager.GetTrackingKey(polygonTest.activeKey).pressed) {
            var mouse = inputManager.GetTrackingMouse();
            polygonTest.drawable.pos = mouse.pos.ToVector2();
        }
        polygonTest.drawable.settledFrame = polygonTest.showingPolygon;
    }

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