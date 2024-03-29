﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO:  there probably is the possibility to remove this script or to change it a little. See how it's possible.
// only used at init time. Then the voxels are on their own.
public class VoxelsJoint : MonoBehaviour
{

    public int x,y,z;
    public GameObject[,,] neighbors;

    // --------------------- Placement functions ------------------- // 

    public void Place(GameObject copy, Vector3 coordinates, GameObject parent)
    {   
        copy.transform.position = coordinates; // new coordinatesx
        copy.transform.parent = parent.transform;
        // copy.transform.rotation = orientation;
    }

    public Vector3 NewCoordinates(int i, int j, int k, GameObject[,,] neighbors){
        // we add objects in neightboors first with the x axis, then the y and finally the z one.
        // be careful!
        // vector = Quaternion.AngleAxis(-45, Vector3.up) * vector;
        if (i != 0){
            GameObject tmp = neighbors[i-1,j,k];
            Vector3 x = new Vector3(tmp.transform.localScale.x,0,0);
            return tmp.transform.position + x;
        }
        else {
            if (j != 0){
                GameObject tmp = neighbors[i,j-1,k];
                return tmp.transform.position + new Vector3(0,tmp.transform.localScale.y,0);
                }
            else {
                if (k != 0){
                    GameObject tmp = neighbors[i,j,k-1];
                    return tmp.transform.position + new Vector3(0,0,tmp.transform.localScale.z);
                }
                else {
                    return this.transform.position;
                }   
            }
        }
    } 

    // --------------------- INIT functions ------------------------ //
    // first function is called in the pipeline generator, the next in the start function
    /*
    public void Init(int x, int y, int z){
        // initializes the size of the full body + the neighbors + the voxelclass
        this.x = x;
        this.y = y;
        this.z = z;
        this.neighbors = new GameObject[x,y,z]; // careful, the order is not the logical one
        for (int k=0; k< z; k++){
            for (int j=0; j< y; j++){
                for (int i=0; i< x; i++){
                    GameObject child = this.transform.Find(i.ToString() + "." + j.ToString() + "." + k.ToString()).gameObject; // we select all the given children of our parent.
                    if (child.GetComponent<Voxel>()){
                        this.neighbors[i,j,k] = child; // and we assign it in the matrix. 
                    }
                }
            }   
        }
    }

    // we completely change paradigm. 
    // init neighbors :
    public void InitNeighbors(){
        // in a first time, we forget about rotations (or let's say it's in the local view)
        // we start by checking objets in the x axis, then z, then y = [x+,x-,y+,y-,z+,z-]
        for (int k=0; k< z; k++){
            for (int j=0; j< y; j++){
                for (int i=0; i< x; i++){
                    if (k==0){
                        if (j==0){
                            if (i==0){
                                // first one - 3 neighbors
                                if (x!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                }
                                if (y!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                }
                                if (z!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                }                       
                                //the rest is at null by default
                            }
                            else if (i< x-1) {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                if (y!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                }
                                if (z!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                }  
                            }
                            else {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                if (y!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                }
                                if (z!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                }  
                            }
                        }
                        else if (j< y-1){
                            if (i==0){
                                if (x!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                }        
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                if (z!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                }
                                
                            }
                            else if (i< x-1) {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                if (z!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                }
                            }
                            else {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                if (z!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                }
                            }
                        }
                        else {
                            if (i==0){
                                if (x!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                }
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                if (z!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                } 
                            }
                            else if (i< x-1) {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                if (z!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                }
                            }
                            else {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                if (z!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                }
                            }
                        }
                    }
                    else if (k<z-1) {
                        if (j==0){
                            if (i==0){
                                if (x!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                }
                                if (y!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                }      
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                            }
                            else if (i< x-1) {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                if (y!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                }  
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                            }
                            else {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                if (y!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                }  
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                            }
                        }
                        else if (j< y-1){
                            if (i==0){
                                if (x!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                }
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                            }
                            else if (i< x-1) {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                            }
                            else {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                            }
                        }
                        else {
                            if (i==0){
                                if (x!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                }
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                            }
                            else if (i< x-1) {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z- 
                            }
                            else {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(4,this.neighbors[i,j,k+1]); // z+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                            }
                        }
                    }
                    else {
                        if (j==0){
                            if (i==0){
                                if (x!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                }
                                if (y!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                }  
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                            }
                            else if (i< x-1) {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                if (y!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                } 
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                            }
                            else {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                if (y!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                }  
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                            }
                        }
                        else if (j< y-1){
                            if (i==0){
                                if (x!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                }
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                                
                            }
                            else if (i< x-1) {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                            }
                            else {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(2,this.neighbors[i,j+1,k]); // y+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                            }
                        }
                        else {
                            if (i==0){
                                if (x!=1){
                                    this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                }
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                                
                            }
                            else if (i< x-1) {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(0,this.neighbors[i+1,j,k]); // x+
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                                
                            }
                            else {
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(1,this.neighbors[i-1,j,k]); // x-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(3,this.neighbors[i,j-1,k]); // y-
                                this.neighbors[i,j,k].GetComponent<Voxel>().SetNeighbor(5,this.neighbors[i,j,k-1]); // z-
                            }
                        }
                    }
                }
            }
        }
    }
    */
}
