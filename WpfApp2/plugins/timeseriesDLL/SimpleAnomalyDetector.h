#pragma once
#ifndef SIMPLEANOMALYDETECTOR_H_
#define SIMPLEANOMALYDETECTOR_H_

#include "anomaly_detection_util.h"
#include "AnomalyDetector.h"
#include <vector>
#include <algorithm>
#include <string.h>
#include <math.h>
 // mabey move out to make the code elegant
class VectorWrapper {
public:
	std::vector<AnomalyReport> anomalyVec;
	//think....
	VectorWrapper() {}
	~VectorWrapper() {}
	
};

struct correlatedFeatures {
	string feature1, feature2;  // names of the correlated features
	float corrlation;
	Line lin_reg;
	float threshold;
	float cx, cy;
};


class SimpleAnomalyDetector :public TimeSeriesAnomalyDetector {
protected:
	vector<correlatedFeatures> cf;
	float threshold;
public:
	SimpleAnomalyDetector();
	virtual ~SimpleAnomalyDetector();

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
	virtual void learnHelper(const TimeSeries& ts, float p/*pearson*/, string f1, string f2, Point** ps);
	virtual bool isAnomalous(float x, float y, correlatedFeatures c);
	Point** toPoints(vector<float> x, vector<float> y);
	float findThreshold(Point** ps, size_t len, Line rl);
};

//create SimpleAnomalyDetector
extern "C" __declspec(dllexport) void* Create() {
	return (void*) new SimpleAnomalyDetector();
}

//learn normal by creating time series
extern "C" __declspec(dllexport) void learnnig(SimpleAnomalyDetector * sad, const char* CSVfileName) {
	TimeSeries ts(CSVfileName);
	sad->learnNormal(ts);
	return;
}

//detect anomalies by creating time series
extern "C" __declspec(dllexport) void detecting(SimpleAnomalyDetector * sad, VectorWrapper * wrapAR, const char* detectfileName) {
	//create TimeSeries for anomaly file
	TimeSeries ts(detectfileName);
	//get anomaly report vector from detect func
	vector<AnomalyReport>  ar = sad->detect(ts);
	// insert the anomaly report to wraper vector 
	wrapAR->anomalyVec = ar;
}

/// extern func to vector rapper
extern "C" __declspec(dllexport) void* CreateVectorWrapper() {
	return (void*) new VectorWrapper;
}

extern "C" __declspec(dllexport) int vectorSize(VectorWrapper* v) {
	return v->anomalyVec.size();
}

extern "C" __declspec(dllexport) long getTS(VectorWrapper* v, int index) {
	return v->anomalyVec[index].timeStep;
}

extern "C" __declspec(dllexport) void getDP(VectorWrapper* v, int index, char *str) {
		strcpy (str, v->anomalyVec[index].description.c_str);
}

extern "C" __declspec(dllexport) int getDPLen(VectorWrapper* v, int index) {
	return v->anomalyVec[index].description.size();
}
#endif /* SIMPLEANOMALYDETECTOR_H_ */