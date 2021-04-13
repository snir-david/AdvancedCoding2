#pragma once
#pragma once
#ifndef CIRCLEANOMALYDETECTOR_H_
#define CIRCLEANOMALYDETECTOR_H_

#include "anomaly_detection_util.h"
#include "AnomalyDetector.h"
#include "minCircle.h"
#include <vector>
#include <algorithm>
#include <string.h>
#include <math.h>
// mabey move out to make the code elegant
class VectorWrapper {
public:
	std::vector<AnomalyReport> anomalyVec;
	VectorWrapper() {}
	~VectorWrapper() {}
};

struct correlatedFeatures {
	string feature1, feature2;  // names of the correlated features
	float corrlation;
	Point circleCenter;
	float threshold;
};


class CircleAnomalyDetector {
protected:
	vector<correlatedFeatures> cf;
	float threshold;
public:
	TimeSeries tsCSV;
	Circle minCirc;
	CircleAnomalyDetector();
	virtual ~CircleAnomalyDetector();

	virtual void learnNormal(const TimeSeries& ts);
	virtual vector<AnomalyReport> detect(const TimeSeries& ts);
	vector<correlatedFeatures> getNormalModel() {
		return cf;
	}
	void setCorrelationThreshold(float threshold) {
		this->threshold = threshold;
	}

	// helper methods
protected:
	virtual void learnHelper(size_t len, float p/*pearson*/, string f1, string f2, Point** ps);
	virtual bool isAnomalous(Point x, Point y, correlatedFeatures c);
	Point** toPoints(vector<float> x, vector<float> y);
};

//create CircleAnomalyDetector 
extern "C" __declspec(dllexport) void* Create() {
	return (void*) new CircleAnomalyDetector();
}

//learn normal by creating time series
extern "C" __declspec(dllexport) void learnnig(CircleAnomalyDetector * cad, const char* CSVfileName) {
	TimeSeries ts(CSVfileName);
	cad->learnNormal(ts);
	return;
}

//detect anomalies by creating time series
extern "C" __declspec(dllexport) void detecting(CircleAnomalyDetector * cad, VectorWrapper * wrapAR, const char* detectfileName) {
	//create TimeSeries for anomaly file
	TimeSeries ts(detectfileName);
	//get anomaly report vector from detect func
	vector<AnomalyReport>  ar = cad->detect(ts);
	// insert the anomaly report to wraper vector 
	wrapAR->anomalyVec = ar;
}

/// extern func to vector rapper
extern "C" __declspec(dllexport) void* CreateVectorWrapper() {
	return (void*) new VectorWrapper;
}

extern "C" __declspec(dllexport) int vectorSize(VectorWrapper * v) {
	return v->anomalyVec.size();
}

extern "C" __declspec(dllexport) long getTS(VectorWrapper * v, int index) {
	return v->anomalyVec[index].timeStep;
}

extern "C" __declspec(dllexport) void getDP(VectorWrapper * v, int index, char* str) {
	strcpy(str, v->anomalyVec[index].description.c_str());
}

extern "C" __declspec(dllexport) int getDPLen(VectorWrapper * v, int index) {
	return v->anomalyVec[index].description.size();
}

extern "C" __declspec(dllexport) void findMinCircle( CircleAnomalyDetector * cad, int attIdx, int corrIdx) {
	vector<float> att = cad->tsCSV.getAttributeDataIdx(attIdx);
	vector<float> corr = cad->tsCSV.getAttributeDataIdx(corrIdx);
	Point** p = new Point *[att.size()];
	for (int i = 0; i < att.size(); i++)
	{
		p[i] = new Point(att[i], corr[i]);
	}
	cad->minCirc= findMinCircle(p, att.size());
	for (int i = 0; i < att.size(); i++)
	{
		delete p[i];
	}
	delete []p;
}

extern "C" __declspec(dllexport) double getRadius(CircleAnomalyDetector * cad) {
	return cad->minCirc.radius;
}
extern "C" __declspec(dllexport) double getCenterX(CircleAnomalyDetector * cad) {
	return cad->minCirc.center.x;
}
extern "C" __declspec(dllexport) double getCenterY(CircleAnomalyDetector * cad) {
	return cad->minCirc.center.y;
}
#endif /* CIRCLEANOMALYDETECTOR_H_ */